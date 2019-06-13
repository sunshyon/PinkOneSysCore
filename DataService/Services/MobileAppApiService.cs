using DataAccess;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utility;

namespace DataService
{
    public class MobileAppApiService:IMobileAppApiService
    {
        #region 基础部分
        private ModelJsonRet _mjr;
        public ModelWxSetting mWxSetting = JsonFileProvider.Instance.GetSettings<ModelWxSetting>();
        /// <summary>
        /// 获取数据单元模块
        /// </summary>
        public IUnitOfWork UnitOfWork
        {
            get
            {
                GC.Collect();
                //实现一个HttpContext请求管道对应唯一UnitOfWork对象，继而对应唯一DbContext对象,以避免各种DbContext线程安全Bug
                //由于Http用完即销毁，UnitOfWork、DbContext对象也随之销毁。避免DbContext占用内存
                IUnitOfWork _unitOfWork = (UnitOfWork)HttpContextCore.GetItem("MAppUnitOfWork");
                if (_unitOfWork == null)
                {
                    _unitOfWork = new UnitOfWork();
                    HttpContextCore.AddItem("MAppUnitOfWork", _unitOfWork);
                }
                return _unitOfWork;
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public MobileAppApiService()
        {
            _mjr = new ModelJsonRet()
            {
                code = 0,
                content = "",
                errMsg = ""
            };
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion

        public ModelJsonRet UploadAttFromApp(int schoolId,long stuId,byte attType,byte attWay,byte isSendMsg)
        {
            _mjr.errMsg = "未找到匹配的项";
            var school = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID == schoolId).Result.FirstOrDefault();
            var stuObj = UnitOfWork.Repository<SYS_Student>().GetEntitiesAsync(x => x.SchoolId == schoolId && x.ID == stuId&&x.Status==(byte)StuStatus.正常).Result.FirstOrDefault();
            if (null == stuObj)
            {
                _mjr.errMsg = "未找到学生ID";
                return _mjr;
            }
            var currentAttStatus = CurrentAttStatus.在校;
            //判断是签入还是签出
            if (attType==(byte)AttType.签出)
            {
                currentAttStatus = CurrentAttStatus.离校;
            }

            SYS_StudentAttRecord stuAttRecord = new SYS_StudentAttRecord
            {
                SchoolId = stuObj.SchoolId,
                MasterId = stuId,
                AttWay = attWay,
                AttType = (byte)attType,
                AttTime = DateTime.Now,
                AttTimeStr = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
            };
            UnitOfWork.Repository<SYS_StudentAttRecord>().AddEntity(stuAttRecord);
            var addOk = UnitOfWork.CommitAsync().Result;
            if (addOk > 0)
            {
                _mjr.code = 1;
                _mjr.errMsg = "";
                _mjr.content = "OK";

                //更新学生当前考勤状态
                stuObj.AttStatus = (byte)currentAttStatus;
                UnitOfWork.Repository<SYS_Student>().UpdateEntity(stuObj);

                if (isSendMsg > 0)
                {
                    //推送考勤消息
                    List<FK_Stu_Parent> fks = UnitOfWork.Repository<FK_Stu_Parent>().GetEntitiesAsync(x => x.SchoolId == stuObj.SchoolId && x.StuId == stuObj.ID).Result;//查找学生家长
                    if (null != fks && fks.Count > 0)
                    {
                        var pWxPInfo = UnitOfWork.Repository<Wx_PublicInfo>().GetEntitiesAsync(x => x.Type == 1).Result.FirstOrDefault();
                        var wxpInfoId = pWxPInfo == null ? 0 : pWxPInfo.ID;
                        if (school.WxPublicInfoId != null && school.WxPublicInfoId > 0)
                            wxpInfoId = (int)school.WxPublicInfoId;
                        var wxPubInfo = UnitOfWork.Repository<Wx_PublicInfo>().GetEntitiesAsync(x => x.ID == wxpInfoId).Result.FirstOrDefault();
                        foreach (var fk in fks)
                        {
                            //推送消息
                            if (null != wxPubInfo)
                            {
                                // 需要从Cookie中获取
                                ModelWxMsg<ModelWmAttendance> wxMsg = new ModelWxMsg<ModelWmAttendance>();
                                wxMsg = JsonFileProvider.Instance.GetSettings<ModelWxMsg<ModelWmAttendance>>();
                                wxMsg.data.first.value = string.Format(wxMsg.data.first.value, (AttType)attType);
                                wxMsg.url = mWxSetting.PubUrl_Host + "/WxRelated/Center/AttDetail?attId=" + stuAttRecord.ID;
                                wxMsg.touser = fk.OpenId;
                                wxMsg.data.keyword1.value = stuObj.StuName;
                                wxMsg.data.keyword2.value = DateTime.Now.ToString();
                                wxMsg.data.keyword3.value = school.SchoolName;
                                ModelWmResult wmResult = WXOAuthApiHelper.SendTmplMessage(wxPubInfo.AccessToken, wxMsg);
                                if (null == wmResult || wmResult.msgid <= 0)
                                {
                                    LogHelper.Error("推送错误:" + wmResult.errmsg);
                                }
                            }
                        }
                    }
                }
            }
            return _mjr;
        }

        /// <summary>
        /// app登录type 1：staff，2：parent
        /// </summary>
        public ModelJsonRet MobileAppLogin(byte type, string username ,string password)
        {
            _mjr.errMsg = "用户名或密码错误";
            if (type == 1)
            {
                var staff = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.PinkoneAccount == username && x.PinkonePassword == password).Result.FirstOrDefault();
                if (staff != null)
                {
                    var school = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID == staff.SchoolId).Result.FirstOrDefault();
                    if (school != null && school.AvatarPic != null)
                    {
                        school.AvatarPic = null;
                    }
                    var classes = UnitOfWork.Repository<SYS_Class>().GetEntitiesAsync(x => x.SchoolId == staff.SchoolId).Result;
                    var students = UnitOfWork.Repository<SYS_Student>().GetEntitiesAsync(x => x.SchoolId == staff.SchoolId&&x.Status==(byte)StuStatus.正常).Result;
                    var staffs = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.SchoolId == staff.SchoolId && x.Status == (byte)StaffStatus.在职).Result;
                    //var notices=UnitOfWork.Repository<SYS_Notice>().GetEntitiesAsync(x=>x.SchoolId==staff.SchoolId&&x.)

                    var json = new
                    {
                        type = type,
                        user = JsonHelper.ObjToObj<ModelMobileAppStaff>(staff),
                        school = JsonHelper.ObjToObj<ModelMobileAppSchool>(school),
                        classes = classes,
                        students = JsonHelper.ObjToObj<List<ModelMobileAppStu>>(students),
                        staffs = JsonHelper.ObjToObj<List<ModelMobileAppStaff>>(staffs)
                    };
                    _mjr.code = 1;
                    _mjr.errMsg = "";
                    _mjr.content = json;
                }
            }
            else
            {
                //var parent= UnitOfWork.Repository<SYS_Parent>().GetEntitiesAsync(x => x.PinkoneAccount == username && x.PinkonePassword == password).Result.FirstOrDefault();
            }

            return _mjr;
        }
    }
}
