using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataService
{
    public class AttendanceService : BaseService, IAttendanceService
    {
        #region 学生考勤相关
        /// <summary>
        /// 班级签到情况概览
        /// </summary>
        public string GetClassesAttNow()
        {
            var res = "";
            List<SYS_Student> allStus = (List<SYS_Student>)GetSchoolEntities("stu");
            List<SYS_Class> classes = (List<SYS_Class>)GetSchoolEntities("class");

            if (allStus.Count > 0 || classes.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var item in classes)
                {
                    var cStus = allStus.Where(x => x.ClassId == item.ID).ToList();
                    var inSchCount = cStus.Where(x => x.AttStatus == (byte)CurrentAttStatus.在校).ToList().Count();
                    var leaveSchCount = cStus.Where(x => x.AttStatus == (byte)CurrentAttStatus.离校).ToList().Count();
                    var noSignCount = cStus.Count() - inSchCount - leaveSchCount;

                    sb.Append("<tr><td>" + item.ClassName + "</td>");
                    sb.Append("<td>" + cStus.Count() + "</td>");
                    var colorStr = string.Empty;
                    if (inSchCount > 0) colorStr = "green";
                    sb.Append("<td style='color:" + colorStr + "'>" + inSchCount + "</td>");
                    sb.Append("<td>" + leaveSchCount + "</td>");
                    if (noSignCount > 0) colorStr = "red";
                    sb.Append("<td style='color:" + colorStr + "'>" + noSignCount + "</td>");
                    sb.Append("<td><button class='btn btn-default btn-xs' onclick='classAttDetail(" + item.ID + ")'><i class='fa fa-th'></i></button></td></tr>");
                }
                res = sb.ToString();
            }
            return res;
        }
        /// <summary>
        /// 学生实时签到情况
        /// </summary>
        public string GetStuAttNow(string nqy, int cqy)
        {
            var res = "";
            List<SYS_Student> allStus = (List<SYS_Student>)GetSchoolEntities("stu");
            if (null != nqy && nqy.Length > 0)
                allStus = allStus.Where(x => x.StuName.Contains(nqy)).ToList();
            if (cqy > 0)
                allStus = allStus.Where(x => x.ClassId == cqy).ToList();
            List<SYS_Class> classes = (List<SYS_Class>)GetSchoolEntities("class");

            if (allStus.Count > 0 || classes.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                var tickStr = "<i class='fa fa-check'></i>";
                foreach (var item in classes)
                {
                    var cStus = allStus.Where(x => x.ClassId == item.ID).ToList();
                    foreach (var s in cStus)
                    {
                        sb.Append("<tr><td>" + item.ClassName + "</td>");
                        sb.Append("<td>" + s.StuName + "</td>");
                        sb.Append("<td class='text-success'>" + (s.AttStatus == (byte)CurrentAttStatus.在校 ? tickStr : "") + "</td>");
                        sb.Append("<td>" + (s.AttStatus == (byte)CurrentAttStatus.离校 ? tickStr : "") + "</td>");
                        var isNoSign = (s.AttStatus == (byte)CurrentAttStatus.未签到 || s.AttStatus == null);
                        sb.Append("<td class='text-danger'>" + (isNoSign ? tickStr : "") + "</td>");
                        if (!isNoSign && s.AttStatus != null)
                            sb.Append("<td><button class='btn btn-default btn-xs' onclick='stuAttDetail(\"" + s.StuName+ "\")'><i class='fa fa-th'>&ensp;查看</i></button></td></tr>");
                        else
                            sb.Append("<td><button class='btn btn-default btn-xs' onclick='addStuAtt(" + s.ID + ")'>手动签到</button></td></tr>");
                    }
                }
                var json = new
                {
                    stuStr = sb.ToString(),
                    classes = classes
                };
                res = JsonHelper.ToJson(json);
            }
            return res;
        }

        /// <summary>
        /// 手动添加考勤记录
        /// </summary>
        public int AddStuAttManually(long stuId, byte attType, string attTime, string attTemp, string attRemark,byte isSendWxMsg=1)
        {
            var res = 0;
            List<SYS_Student> allStus = (List<SYS_Student>)GetSchoolEntities("stu");
            SYS_Student sObj = allStus.Where(x => x.ID == stuId).FirstOrDefault();
            if (sObj != null)
            {
                DateTime aTime = DateTime.Parse(attTime);
                SYS_StudentAttRecord sar = new SYS_StudentAttRecord()
                {
                    SchoolId = mlUser.School.ID,
                    MasterId = stuId,
                    AttType = attType,
                    AttTime = aTime,
                    AttTimeStr= aTime.ToString("yyyyMMddHHmmssfff"),
                    AttWay = (byte)AttWay.手动添加,
                    Temperature = attTemp,
                    Remark = attRemark
                };
                UnitOfWork.Repository<SYS_StudentAttRecord>().AddEntity(sar);
                var addOk = UnitOfWork.CommitAsync().Result;
                if (addOk > 0)
                {
                    //修改考勤状态
                    if (attType == (byte)AttType.签出)
                        sObj.AttStatus = (byte)CurrentAttStatus.离校;
                    else
                        sObj.AttStatus = (byte)CurrentAttStatus.在校;
                    UnitOfWork.Repository<SYS_Student>().UpdateEntity(sObj);

                    //推送
                    if (isSendWxMsg == 1)
                    {
                        //查找学生家长
                        List<FK_Stu_Parent> fks = UnitOfWork.Repository<FK_Stu_Parent>().GetEntitiesAsync(x => x.SchoolId == sObj.SchoolId && x.StuId == sObj.ID).Result;
                        if (null != fks && fks.Count > 0)
                        {
                            var pWxPInfo = UnitOfWork.Repository<Wx_PublicInfo>().GetEntitiesAsync(x => x.Type == 1).Result.FirstOrDefault();
                            var wxpInfoId = pWxPInfo == null ? 0 : pWxPInfo.ID;
                            if (mlUser.School.WxPublicInfoId != null && mlUser.School.WxPublicInfoId > 0)
                                wxpInfoId = (int)mlUser.School.WxPublicInfoId;
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
                                    wxMsg.touser = fk.OpenId;
                                    wxMsg.url = mWxSetting.PubUrl_Host + "/WxRelated/Center/AttDetail?attId=" + sar.ID;
                                    wxMsg.data.keyword1.value = sObj.StuName;
                                    wxMsg.data.keyword2.value = DateTime.Now.ToString();
                                    wxMsg.data.keyword3.value = mlUser.School.SchoolName;
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
                res = addOk + UnitOfWork.CommitAsync().Result;
                
            }
            return res;
        }
        /// <summary>
        /// 获取学生详细考勤记录
        /// </summary>
        public string GetStuAttDetails(string nqy, int cqy, string sTime, string eTime)
        {
            var res = "";

            List<SYS_Class> classes = (List<SYS_Class>)GetSchoolEntities("class");
            List<SYS_Student> allStus = (List<SYS_Student>)GetSchoolEntities("stu");
            if (null != nqy && nqy.Length > 0)
                allStus = allStus.Where(x => x.StuName.Contains(nqy)).ToList();
            if (cqy > 0)
                allStus = allStus.Where(x => x.ClassId == cqy).ToList();
            sTime += " 00:00:00";
            eTime += " 23:59:59";
            DateTime startTime = DateTime.Parse(sTime);
            DateTime endTime = DateTime.Parse(eTime);
            List<SYS_StudentAttRecord> stuAtts = UnitOfWork.Repository<SYS_StudentAttRecord>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID
            && x.AttTime > startTime && x.AttTime < endTime).Result.OrderBy(x => x.AttTime).ToList();
            //List<SYS_StudentAttRecord> stuAtts = UnitOfWork.Repository<SYS_StudentAttRecord>().GetEntities(x => x.SchoolId == mlUser.School.ID).OrderBy(x => x.AttTime).ToList();
            if (allStus.Count > 0 || classes.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                if (stuAtts.Count == 0)
                {
                    sb.Append("<tr><td class='text-danger' colspan='6'>未有记录</td></tr>");
                }
                foreach (var item in classes)
                {
                    var cStus = allStus.Where(x => x.ClassId == item.ID).ToList();
                    var resList = (from cs in cStus
                                   join sa in stuAtts on cs.ID equals sa.MasterId
                                   select new ModelStudentWithAtt()
                                   {
                                       Stu = cs,
                                       StuAtt = sa
                                   }).ToList();
                    foreach (var s in resList)
                    {
                        sb.Append("<tr><td>" + item.ClassName + "</td>");
                        sb.Append("<td>" + s.Stu.StuName + "</td>");
                        sb.Append("<td>" + s.StuAtt.AttTime + "</td>");
                        sb.Append("<td>" + s.StuAtt.Temperature + "</td>");
                        sb.Append("<td>" + (AttType)s.StuAtt.AttType + "</td>");
                        sb.Append("<td>" + (AttWay)s.StuAtt.AttWay + "</td>");
                    }
                }
                var json = new
                {
                    stuStr = sb.ToString(),
                    classes = classes
                };
                res = JsonHelper.ToJson(json);
            }
            return res;
        }
        /// <summary>
        /// 按月展示考勤信息
        /// </summary>
        /// <returns></returns>
        public string GetStuAttMouth(int cqy, string mouth)
        {
            var res = "";
            List<SYS_Class> classes = (List<SYS_Class>)GetSchoolEntities("class");
            List<SYS_Student> allStus = (List<SYS_Student>)GetSchoolEntities("stu");
            if (cqy > 0)
                allStus = allStus.Where(x => x.ClassId == cqy).ToList();

            DateTime startMouth = DateTime.Parse(mouth);
            int days = DateTime.DaysInMonth(startMouth.Year, startMouth.Month);
            DateTime endMouth = startMouth.AddDays(days);
            List<SYS_StudentAttRecord> stuAtts = UnitOfWork.Repository<SYS_StudentAttRecord>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID
            && x.AttTime > startMouth && x.AttTime < endMouth).Result.OrderBy(x => x.AttTime).ToList();

            if (classes.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("<thead><tr><th>姓名</th>");
                for (var i = 1; i <= days; i++)
                {
                    sb.Append("<th>" + i + "</th>");
                }
                sb.Append("<th>合计</th></tr></thead>");
                if (allStus.Count > 0)
                {
                    sb.Append("<tbody>");
                    foreach (var item in classes)
                    {
                        var cStus = allStus.Where(x => x.ClassId == item.ID).ToList();
                        //var resList = (from cs in cStus
                        //               join sa in stuAtts on cs.ID equals sa.MasterId
                        //               select cs).ToList();
                        foreach (var s in cStus)
                        {
                            var attCount = 0;
                            sb.Append("<tr><td>" + s.StuName + "</td>");
                            for (var i = 1; i <= days; i++)
                            {
                                var startDay = startMouth.AddDays(i - 1);
                                var endDay = startMouth.AddDays(i);
                                var dayAtt = stuAtts.Where(x => x.MasterId == s.ID && x.AttTime > startDay && x.AttTime < endDay).FirstOrDefault();
                                var str = "×";
                                if (null != dayAtt)
                                {
                                    str = "√";
                                    attCount++;
                                    if(dayAtt.AttWay==(byte)AttWay.手动添加)
                                        str = "△";
                                }
                                if(startDay.DayOfWeek==DayOfWeek.Saturday)
                                    str= "休";
                                if (startDay.DayOfWeek == DayOfWeek.Sunday)
                                    str = "休";
                                sb.Append("<td>" + str + "</td>");
                            }
                            sb.Append("<td>" + attCount + "</td></tr>");
                        }
                    }
                    sb.Append("</tbody>");
                }
                var json = new
                {
                    stuStr = sb.ToString(),
                    classes = classes
                };
                res = JsonHelper.ToJson(json);
            }
            return res;
        }
        #endregion

        #region 职员考勤相关
        /// <summary>
        /// 职员实时签到情况
        /// </summary>
        public string GetStaffAttNow(string nqy)
        {
            var res = "";
            List<SYS_Staff> allStaffs = (List<SYS_Staff>)GetSchoolEntities("staff");
            if (null != nqy && nqy.Length > 0)
                allStaffs = allStaffs.Where(x => x.StaffName.Contains(nqy)).ToList();

            if (allStaffs.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                var tickStr = "<i class='fa fa-check'></i>";
                var staffRoles = UnitOfWork.Repository<SYS_StaffRole>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID||x.SchoolId==0).Result.OrderBy(x=>x.RoleLevel).ToList();
                foreach (var item in allStaffs)
                {
                    var role = staffRoles.Where(x => x.ID == item.RoleId).FirstOrDefault();
                    sb.Append("<tr><td>" + item.StaffName + "</td>");
                    sb.Append("<td>" + (role==null?"未填":role.RoleName )+ "</td>");
                    sb.Append("<td class='text-success'>" + (item.AttStatus == (byte)CurrentAttStatus.在校 ? tickStr : "") + "</td>");
                    sb.Append("<td>" + (item.AttStatus == (byte)CurrentAttStatus.离校 ? tickStr : "") + "</td>");
                    var isNoSign = (item.AttStatus == (byte)CurrentAttStatus.未签到 || item.AttStatus == null);
                    sb.Append("<td class='text-danger'>" + (isNoSign ? tickStr : "") + "</td>");
                    if (!isNoSign && item.AttStatus != null)
                        sb.Append("<td><button class='btn btn-default btn-xs' onclick='staffAttDetail(\"" + item.StaffName + "\")'><i class='fa fa-th'>&ensp;查看</i></button></td></tr>");
                    else
                        sb.Append("<td><button class='btn btn-default btn-xs' onclick='addStaffAtt(" + item.ID + ")'>手动签到</button></td></tr>");

                }
                res = sb.ToString();
            }
            return res;
        }

        /// <summary>
        /// 手动添加考勤记录
        /// </summary>
        public int AddStaffAttManually(int staffId, byte attType, string attTime, string attRemark)
        {
            var res = 0;
            List<SYS_Staff> allStaffs = (List<SYS_Staff>)GetSchoolEntities("staff");
            SYS_Staff sObj = allStaffs.Where(x => x.ID == staffId).FirstOrDefault();
            if (sObj != null)
            {
                DateTime aTime = DateTime.Parse(attTime);
                SYS_StaffAttRecord sar = new SYS_StaffAttRecord()
                {
                    SchoolId = mlUser.School.ID,
                    MasterId = staffId,
                    AttType = attType,
                    AttTime = aTime,
                    AttTimeStr = aTime.ToString("yyyyMMddHHmmssfff"),
                    AttWay = (byte)AttWay.手动添加,
                    Remark = attRemark
                };
                UnitOfWork.Repository<SYS_StaffAttRecord>().AddEntity(sar);

                //修改考勤状态
                if (attType == (byte)AttType.签出)
                    sObj.AttStatus = (byte)CurrentAttStatus.离校;
                else
                    sObj.AttStatus = (byte)CurrentAttStatus.在校;
                UnitOfWork.Repository<SYS_Staff>().UpdateEntity(sObj);

                res = UnitOfWork.CommitAsync().Result;
             
            }
            return res;
        }
        /// <summary>
        /// 获取职员详细考勤记录
        /// </summary>
        public string GetStaffAttDetails(string nqy, string sTime, string eTime)
        {
            var res = "";

            List<SYS_Staff> allStaffs = (List<SYS_Staff>)GetSchoolEntities("staff");
            if (null != nqy && nqy.Length > 0)
                allStaffs = allStaffs.Where(x => x.StaffName.Contains(nqy)).ToList();
            sTime += " 00:00:00";
            eTime += " 23:59:59";
            DateTime startTime = DateTime.Parse(sTime);
            DateTime endTime = DateTime.Parse(eTime);
            List<SYS_StaffAttRecord> staffAtts = UnitOfWork.Repository<SYS_StaffAttRecord>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID
            && x.AttTime > startTime && x.AttTime < endTime).Result.OrderBy(x => x.AttTime).ToList();
            if (allStaffs.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                if (staffAtts.Count == 0)
                {
                    sb.Append("<tr><td class='text-danger' colspan='5'>未有记录</td></tr>");
                }

                var resList = (from s in allStaffs
                               join sa in staffAtts on s.ID equals sa.MasterId
                               select new ModelStaffWithAtt()
                               {
                                   Staff = s,
                                   StaffAtt = sa
                               }).ToList();
                var staffRoles = UnitOfWork.Repository<SYS_StaffRole>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID || x.SchoolId == 0).Result.OrderBy(x => x.RoleLevel).ToList();
                foreach (var s in resList)
                {
                    var role = staffRoles.Where(x => x.ID == s.Staff.RoleId).FirstOrDefault();
                    sb.Append("<tr><td>" + s.Staff.StaffName + "</td>");
                    sb.Append("<td>" + (role==null?"未填":role.RoleName) + "</td>");
                    sb.Append("<td>" + s.StaffAtt.AttTime + "</td>");
                    sb.Append("<td>" + (AttType)s.StaffAtt.AttType + "</td>");
                    sb.Append("<td>" + (AttWay)s.StaffAtt.AttWay + "</td>");
                }
                res = sb.ToString();
            }
            return res;
        }
        /// <summary>
        /// 按月展示考勤信息
        /// </summary>
        /// <returns></returns>
        public string GetStaffAttMouth(string mouth)
        {
            var res = "";
            List<SYS_Staff> allStaffs = (List<SYS_Staff>)GetSchoolEntities("staff");

            DateTime startMouth = DateTime.Parse(mouth);
            int days = DateTime.DaysInMonth(startMouth.Year, startMouth.Month);
            DateTime endMouth = startMouth.AddDays(days);
            List<SYS_StaffAttRecord> stuAtts = UnitOfWork.Repository<SYS_StaffAttRecord>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID
            && x.AttTime > startMouth && x.AttTime < endMouth).Result.OrderBy(x => x.AttTime).ToList();


            StringBuilder sb = new StringBuilder();
            sb.Append("<thead><tr><td>姓名</td>");
            for (var i = 1; i <= days; i++)
            {
                sb.Append("<td>" + i + "</td>");
            }
            sb.Append("<td>合计</td></tr></thead>");
            if (allStaffs.Count > 0)
            {
                sb.Append("<tbody>");
                foreach (var s in allStaffs)
                {
                    var attCount = 0;
                    sb.Append("<tr><td>" + s.StaffName + "</td>");
                    for (var i = 1; i <= days; i++)
                    {
                        var startDay = startMouth.AddDays(i - 1);
                        var endDay = startMouth.AddDays(i);
                        var dayAtt = stuAtts.Where(x => x.MasterId == s.ID && x.AttTime > startDay && x.AttTime < endDay).FirstOrDefault();
                        var str = "×";
                        if (null != dayAtt)
                        {
                            str = "√";
                            attCount++;
                            if (dayAtt.AttWay == (byte)AttWay.手动添加)
                                str = "△";
                        }
                        if (startDay.DayOfWeek == DayOfWeek.Saturday)
                            str = "休";
                        if (startDay.DayOfWeek == DayOfWeek.Sunday)
                            str = "休";
                        sb.Append("<td>" + str + "</td>");
                    }
                    sb.Append("<td>" + attCount + "</td></tr>");
                }
                sb.Append("</tbody>");

                res = sb.ToString();
            }
            return res;
        }
        #endregion
    }
}
