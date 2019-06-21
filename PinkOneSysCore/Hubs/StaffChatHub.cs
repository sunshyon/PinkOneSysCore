using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Utility;
using Domain;
using DataService;

namespace PinkOneSysCore
{
    public class StaffChatHub:Hub
    {
        /// <summary>
        /// 学校在线用户字典
        /// </summary>
        public static ConcurrentDictionary<int, List<ModelChatUser>> SchoolOnLineUsersDic = new ConcurrentDictionary<int, List<ModelChatUser>>();
        /// <summary>
        /// 学校消息队列字典
        /// </summary>
        public static ConcurrentDictionary<int, ConcurrentQueue<ModelChatObj>> SchoolMsgQueueDic = new ConcurrentDictionary<int, ConcurrentQueue<ModelChatObj>>();


        public async Task SendSchoolMsg(string chatObj)
        {
            var cObj = JsonHelper.JsonToT<ModelChatObj>(chatObj);
            if (cObj != null)
            {
                cObj.Time = DateTime.Now.ToString();
                await Clients.Group("schoolGroup" + cObj.SchoolId).SendAsync("ReceiveMessage", cObj);//发送

                //消息队列
                cObj.GotMsgStaffIds = (List<int>)GetOnlineData(cObj.SchoolId, 1);
                var schoolmsgQueue = SchoolMsgQueueDic.GetValueOrDefault(cObj.SchoolId);
                if (schoolmsgQueue == null)
                {
                    schoolmsgQueue = new ConcurrentQueue<ModelChatObj>();
                }
                schoolmsgQueue.Enqueue(cObj);
                SchoolMsgQueueDic.AddOrUpdate(cObj.SchoolId, schoolmsgQueue,(key,value)=> schoolmsgQueue);
            }
        }

        //客户端查看是否有某用户消息
        public async Task CheckUserMsg()
        {
            var mlUser = JsonHelper.JsonToT<ModelLoginUser>(HttpContextCore.GetCookie(ComConst.UserLogin));
            if (mlUser != null && mlUser.Staff != null)
            {
                var schoolId = mlUser.School.ID;
                var staffId = mlUser.Staff.ID;
                var schoolmsgQueue = SchoolMsgQueueDic.GetValueOrDefault(schoolId);
                if (schoolmsgQueue != null && schoolmsgQueue.Count > 0)
                {
                    var newMsgCount = 0;
                    foreach (var cObj in schoolmsgQueue)
                    {
                        if (!cObj.GotMsgStaffIds.Contains(staffId))//未收到
                        {
                            newMsgCount++;
                        }
                    }
                    if (newMsgCount > 0)
                    {
                        await Clients.Caller.SendAsync("haveNewMsg", newMsgCount);//调用客户端方法
                    }
                }
            }
        }

        public override async Task OnConnectedAsync()
        {
            var mlUser = JsonHelper.JsonToT<ModelLoginUser>(HttpContextCore.GetCookie(ComConst.UserLogin));
            if (mlUser != null && mlUser.Staff != null)
            {
                var schoolId = mlUser.School.ID;
                var staffId = mlUser.Staff.ID;
                string cnnId = Context.ConnectionId;

                //在线用户，以学校为单位
                var mcu = new ModelChatUser
                {
                    CnnId = cnnId,
                    MLUser = mlUser
                };
                var schoolMcUsers = SchoolOnLineUsersDic.GetValueOrDefault(schoolId);
                if (schoolMcUsers == null)
                    schoolMcUsers = new List<ModelChatUser>();
                schoolMcUsers.Add(mcu);
                SchoolOnLineUsersDic.AddOrUpdate(schoolId, schoolMcUsers, (key, value) => schoolMcUsers);
                await Clients.All.SendAsync("chatUserChange", GetOnlineData(schoolId));//更新客户端在线状态

                //客户端以学校分组
                await Groups.AddToGroupAsync(cnnId, "schoolGroup" + schoolId);

                //发送离线消息
                HandleOfflineMsg(schoolId, staffId, cnnId);
            }
            await base.OnConnectedAsync();
        }
        //发送离线消息
        private void HandleOfflineMsg(int schoolId,int staffId,string cnnId)
        {
            var ass = new AssistantService();
            var haStaffs = ass.GetHasAcutStaffs();
            var schoolmsgQueue = SchoolMsgQueueDic.GetValueOrDefault(schoolId);
            if (schoolmsgQueue != null&& schoolmsgQueue.Count>0)
            {
                foreach (var cObj in schoolmsgQueue)
                {
                    //是否过期
                    var msgTime = DateTime.Parse(cObj.Time);
                    var ts = DateTime.Now - msgTime;
                    if (msgTime != null && ts.TotalDays > 2)
                    {
                        schoolmsgQueue.TryDequeue(out ModelChatObj value);
                    }
                    //是否已收到
                    var gmStaffIds = cObj.GotMsgStaffIds;
                    if (gmStaffIds.Contains(staffId))//已收到
                    {
                        if(haStaffs.Count<= gmStaffIds.Count)
                        {
                            schoolmsgQueue.TryDequeue(out ModelChatObj value);
                        }
                    }
                    else
                    {
                        Clients.Caller.SendAsync("ReceiveMessage", cObj);//发送
                        cObj.GotMsgStaffIds.Add(staffId);
                    }
                }
                SchoolMsgQueueDic.AddOrUpdate(schoolId, schoolmsgQueue, (key, value) => schoolmsgQueue);
                if (schoolmsgQueue.Count <= 0)
                    SchoolMsgQueueDic.TryRemove(schoolId ,out schoolmsgQueue);
            }
        }

        public override async Task OnDisconnectedAsync(Exception ex)
        {
            var mlUser = JsonHelper.JsonToT<ModelLoginUser>(HttpContextCore.GetCookie(ComConst.UserLogin));
            var schoolId = mlUser.School.ID;
            string cnnId = Context.ConnectionId;

            //在线用户，以学校为单位
            var schoolMcUsers = SchoolOnLineUsersDic.GetValueOrDefault(schoolId);
            if (schoolMcUsers != null)
            {
                var mcu = schoolMcUsers.Find(x => x.CnnId == cnnId);
                schoolMcUsers.Remove(mcu);
                if (schoolMcUsers.Count == 0)
                {
                    SchoolOnLineUsersDic.TryRemove(schoolId, out schoolMcUsers);
                    await Groups.RemoveFromGroupAsync(cnnId, "schoolGroup" + schoolId);//清理学校组
                }
                else
                    SchoolOnLineUsersDic.AddOrUpdate(schoolId, schoolMcUsers, (key, value) => schoolMcUsers);
                await Clients.All.SendAsync("chatUserChange", GetOnlineData(schoolId));//更新客户端在线状态
            }

            await base.OnDisconnectedAsync(ex);
        }

        private void GetHasAcutStaffs()
        {
            
        }
        //获取在线消息
        private object GetOnlineData(int sId,byte flag=0)
        {
            var schoolMcUsers= SchoolOnLineUsersDic.GetValueOrDefault(sId);
            //var schoolIdList=new List<int>();
            var staffIdList = new List<int>();
            foreach(var smcu in schoolMcUsers)
            {
                if (smcu.MLUser.Staff != null)
                {
                    staffIdList.Add(smcu.MLUser.Staff.ID);
                }
            }
            var json = new
            {
                //schoolIdList,
                staffIdList
            };
            if (flag == 0)
                return json;
            else
                return staffIdList;
        }
    }

    public class ModelChatObj
    {
        public int SchoolId { get; set; }
        public string UserName { get; set; }
        public string StaffId { get; set; }
        public string Avatar { get; set; }
        public byte TargetUserType { get; set; }
        public int TargetUserId { get; set; }
        public string Time { get; set; }
        public string Msg { get; set; }
        /// <summary>
        /// 已收到消息的员工ID集合
        /// </summary>
        public List<int> GotMsgStaffIds { get; set; }

    }
    public class ModelChatUser
    {
        public string CnnId { get; set; }
        public ModelLoginUser MLUser { get; set; }
    }

}
