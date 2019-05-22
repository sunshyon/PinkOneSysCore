using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataService
{
    public class WxService:BaseService, IWxService
    {
        public Wx_Setting GetWxSettingFromDb()
        {
            return UnitOfWork.Repository<Wx_Setting>().GetEntitiesAsync(x => true).Result.FirstOrDefault();
        }


        #region 微信个人信息

        public ModelSysWxUser GetSysWxUserModelTest()
        {
            byte userType = 0;
            SYS_School school = null;
           
                school = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID == 1 && x.Status == 1).Result.FirstOrDefault();
            
            var parent = UnitOfWork.Repository<SYS_Parent>().GetEntitiesAsync(x => x.ID == 3 && x.Status == 1).Result.FirstOrDefault();
            var stus = new List<SYS_Student>();
            if (parent != null)
            {
                var fks = UnitOfWork.Repository<FK_Stu_Parent>().GetEntitiesAsync(x => x.ParentId == parent.ID && x.SchoolId == parent.SchoolId).Result.ToList();

                foreach (var fk in fks)
                {
                    var stu = UnitOfWork.Repository<SYS_Student>().GetEntitiesAsync(x => x.ID == fk.StuId).Result.FirstOrDefault();
                    if (stu != null)
                        stus.Add(stu);
                }
            }
            SYS_Staff staff = null; //UnitOfWork.Repository<SYS_Staff>().GetEntities(x => x.OpenId == openId && x.Status == 1).FirstOrDefault();

            var userName = "";
            var avatarPic = "";
            var openId = "";
            var phone = "";
            if (null != parent && null == staff)
            {
                userType = 1;
                userName = parent.NickName;
                avatarPic = parent.AvatarPic;
                openId = parent.OpenId;
                phone = parent.Phone;
            }
            else if (null == parent && null != staff)
            {
                userType = 2;
                userName = staff.NickName;
                avatarPic = staff.AvatarPic;
                openId = staff.OpenId;
                phone = parent.Phone;
            }
            else if (null != parent && null != staff)
            {
                userType = 3;
                userName = parent.NickName;
                avatarPic = parent.AvatarPic;
                openId = parent.OpenId;
                phone = parent.Phone;
            }
            return new ModelSysWxUser
            {
                School = school,
                UserType = userType,
                UserName = userName,
                AvatarPic = avatarPic,
                OpenId = openId,
                Phone = phone,
                StusJson = JsonHelper.ToJson(stus),
            };
        }

        /// <summary>
        /// 更新微信信息
        /// </summary>
        public void UpdateWxUserInfo(ModelWxUserInfo wxUserInfo)
        {
            var parent = UnitOfWork.Repository<SYS_Parent>().GetEntitiesAsync(x => x.OpenId == wxUserInfo.openid && x.Status == 1).Result.FirstOrDefault();
            if (parent != null)
            {
                parent.NickName = wxUserInfo.nickname;
                parent.AvatarPic = wxUserInfo.headimgurl;
                UnitOfWork.Repository<SYS_Parent>().UpdateEntity(parent);
            }
            var staff = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.OpenId == wxUserInfo.openid && x.Status == 1).Result.FirstOrDefault();
            if (staff != null)
            {
                staff.NickName = wxUserInfo.nickname;
                staff.AvatarPic = wxUserInfo.headimgurl;
                UnitOfWork.Repository<SYS_Staff>().UpdateEntity(staff);
            }
            var isOK= UnitOfWork.CommitAsync().Result;
        }

        /// <summary>
        /// 获取该openid下用户信息
        /// </summary>
        /// <param name="sId">学校ID</param>
        public ModelSysWxUser GetSysWxUserModel(int sId,string openId)
        {
            byte userType = 0;
            SYS_School school = null;
            if (sId > 0)
            {
                school = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID == sId && x.Status == 1).Result.FirstOrDefault();
            }
            var parent = UnitOfWork.Repository<SYS_Parent>().GetEntitiesAsync(x => x.OpenId == openId && x.Status == 1).Result.FirstOrDefault();
            var stus = new List<SYS_Student>();
            if (parent != null)
            {
                var fks = UnitOfWork.Repository<FK_Stu_Parent>().GetEntitiesAsync(x => x.ParentId == parent.ID&&x.SchoolId==parent.SchoolId).Result;
                foreach(var fk in fks)
                {
                    var stu = UnitOfWork.Repository<SYS_Student>().GetEntitiesAsync(x => x.ID == fk.StuId && x.Status == (byte)StuStatus.正常).Result.FirstOrDefault();
                    if (stu != null)
                        stus.Add(stu);
                }
                if (stus.Count <= 0)
                {
                    parent = null;
                }
            }
            var staff= UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.OpenId == openId && x.Status == (byte)StaffStatus.在职).Result.FirstOrDefault();

            var userName = "";
            var avatarPic = "";
            var phone = "";
            if (null != parent && null == staff)
            {
                userType = 1;
                userName = parent.NickName;
                avatarPic = parent.AvatarPic;
                phone = parent.Phone;
            }
            else if (null == parent && null != staff)
            {
                userType = 2;
                userName = staff.NickName;
                avatarPic = staff.AvatarPic;
                phone = parent.Phone;
            }
            else if (null != parent && null != staff)
            {
                userType = 3;
                userName = parent.NickName;
                avatarPic = parent.AvatarPic;
                phone = parent.Phone;
            }
            return new ModelSysWxUser
            {
                School = school,
                UserType = userType,
                UserName = userName,
                AvatarPic = avatarPic,
                OpenId = openId,
                Phone = phone,
                StusJson = JsonHelper.ToJson(stus),
            };
        }
        #endregion

        #region 考勤详情
        public string GetStuAttImgPath(long attId)
        {
            var res = "";
            var attObj = UnitOfWork.Repository<SYS_StudentAttRecord>().GetEntitiesAsync(x => x.ID == attId).Result.FirstOrDefault();
            if (null != attObj)
            {
                var filePath = attObj.MonitoringImg;
                if (System.IO.File.Exists(filePath))
                {
                    res = filePath;
                }
            }
            return res;
        }
        /// <summary>
        /// 学生考勤详情
        /// </summary>
        public ModelAttDetail GetStuAttDatail(long attId)
        {
            ModelAttDetail mad = null;
            var attObj = UnitOfWork.Repository<SYS_StudentAttRecord>().GetEntitiesAsync(x => x.ID == attId).Result.FirstOrDefault();
            if (null != attObj)
            {
                var school = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID == attObj.SchoolId).Result.FirstOrDefault();
                var stu = UnitOfWork.Repository<SYS_Student>().GetEntitiesAsync(x => x.ID == attObj.MasterId).Result.FirstOrDefault();
                if (school == null || stu == null)
                    return mad;
                var cls= UnitOfWork.Repository<SYS_Class>().GetEntitiesAsync(x => x.ID == stu.ClassId).Result.FirstOrDefault();
                mad = new ModelAttDetail
                {
                    AttId= attObj.ID,
                    SchoolName = school.SchoolName,
                    StuName = stu.StuName,
                    ClassName = (cls == null ? "未分班级" : cls.ClassName),
                    Time = attObj.AttTime,
                    AttType = ((AttType)attObj.AttType).ToString(),
                    AttWay = ((AttWay)attObj.AttWay).ToString(),
                    AttImgPath= attObj.MonitoringImg
                };
            }
            return mad;
        }
        #endregion

        #region 关联绑定相关

        /// <summary>
        /// 获取已绑定的对象
        /// </summary>
        public string GetWxBindedJson(string openId)
        {
            object json_staff = null;
            var staff = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.OpenId == openId && x.Status == (byte)StaffStatus.在职).Result.FirstOrDefault();
            if (staff != null)
            {
                var firstName = staff.StaffName.Substring(0, 1);
                json_staff = new
                {
                    name = staff.StaffName.Replace(firstName, "*"),
                    type=2,
                    id=staff.ID,
                };
            }
            
            var fks = UnitOfWork.Repository<FK_Stu_Parent>().GetEntitiesAsync(x => x.OpenId == openId && x.Enabled == true).Result;
            var stus = new List<object>();
            if (fks.Count > 0)
            {
                foreach(var fk in fks)
                {
                    var stu = UnitOfWork.Repository<SYS_Student>().GetEntitiesAsync(x => x.ID == fk.StuId && x.Status == (byte)StuStatus.正常).Result.FirstOrDefault();
                    if (stu != null)
                    {
                        var firstName = stu.StuName.Substring(0, 1);
                        var obj = new
                        {
                            name = stu.StuName.Replace(firstName, "*"),
                            type = 1,
                            id = fk.ID,
                        };
                        stus.Add(obj);
                    }
                }
            }

            var json = new
            {
                stus = stus,
                staff = json_staff
            };
            return JsonHelper.ToJson(json);
        }

        /// <summary>
        /// 删除已绑定对象
        /// </summary>
        public int DelWxBinded(byte type , long id)
        {
            if (type == 1)
            {
                var fk = UnitOfWork.Repository<FK_Stu_Parent>().GetEntitiesAsync(x => x.ID==id).Result.FirstOrDefault();
                if (fk != null)
                    UnitOfWork.Repository<FK_Stu_Parent>().DeleteEntity(fk);
            }
            else
            {
                var staff = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.ID == id).Result.FirstOrDefault();
                if (staff != null)
                {
                    staff.OpenId = "";
                    UnitOfWork.Repository<SYS_Staff>().UpdateEntity(staff);
                }
            }
            return UnitOfWork.CommitAsync().Result;
        }

        /// <summary>
        /// 开始绑定
        /// </summary>
        public string DoWxBind(string name, string cardNo, ModelWxUserInfo wxUser)
        {
            var res = "未检索到名下卡片，请确认人员信息及卡片已录入系统";
            var cards = UnitOfWork.Repository<SYS_Card>().GetEntitiesAsync(x => x.CardNo == cardNo && x.Status == (byte)CardStatus.正常).Result;
            if (cards != null && cards.Count > 0)
            {
                object master = null;
                foreach (var c in cards)
                {
                    if (c.CardType == (byte)CardType.学生卡)
                    {
                        master = UnitOfWork.Repository<SYS_Student>().GetEntitiesAsync(x => x.ID == c.CardMasterId&&x.StuName==name &&x.Status==(byte)StuStatus.正常).Result.FirstOrDefault();
                    }
                    else
                    {
                        master = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.ID == c.CardMasterId && x.StaffName == name && x.Status == (byte)StaffStatus.在职).Result.FirstOrDefault();
                    }
                    if (master != null)
                        break;
                }
                if (master != null)
                {
                    res = "关联失败请联系客服";
                    var isOk = 0;
                    //学生家长关联
                    if (master.GetType() == typeof(SYS_Student))
                    {
                        var stu = (SYS_Student)master;
                        var parent = new SYS_Parent();
                        var oldParent = UnitOfWork.Repository<SYS_Parent>().GetEntitiesAsync(x => x.OpenId.Length > 6 && x.OpenId == wxUser.openid).Result.FirstOrDefault();
                        if (oldParent == null)
                        {
                            parent.OpenId = wxUser.openid;
                            parent.NickName = wxUser.nickname;
                            parent.Status = 1;
                            parent.AvatarPic = wxUser.headimgurl;
                            parent.SchoolId = stu.SchoolId;
                            parent.CreatTime = DateTime.Now;
                            parent.Sex = (byte)wxUser.sex;
                            UnitOfWork.Repository<SYS_Parent>().AddEntity(parent);
                            isOk = UnitOfWork.CommitAsync().Result;
                        }
                        else
                        {
                            parent = oldParent;
                            isOk = 1;
                        }
                        if (isOk > 0)
                        {
                            var oldFsp = UnitOfWork.Repository<FK_Stu_Parent>().GetEntitiesAsync(x => x.StuId == stu.ID && x.OpenId == parent.OpenId).Result.FirstOrDefault();
                            if (oldFsp == null)
                            {
                                FK_Stu_Parent fsp = new FK_Stu_Parent()
                                {
                                    StuId = stu.ID,
                                    ParentId = parent.ID,
                                    OpenId = parent.OpenId,
                                    SchoolId = stu.SchoolId,
                                    Enabled = true
                                };
                                UnitOfWork.Repository<FK_Stu_Parent>().AddEntity(fsp);

                                isOk=UnitOfWork.CommitAsync().Result;
                            }
                            if(isOk>0)
                            {
                                res = "OK,关联成功";
                            }
                        }
                    }
                    else //职员关联
                    {
                        var staff = (SYS_Staff)master;
                        staff.OpenId = wxUser.openid;
                        staff.NickName = wxUser.nickname;
                        staff.AvatarPic = wxUser.headimgurl;
                        staff.Sex = (byte)wxUser.sex;
                        UnitOfWork.Repository<SYS_Staff>().UpdateEntity(staff);
                        isOk=UnitOfWork.CommitAsync().Result;
                        if (isOk > 0)
                        {
                            res = "OK,关联成功";
                        }
                    }
                }
                else
                {
                    res = "未找到人员信息";
                }
            }
            return res;
        }
        #endregion

        #region 家长绑定注册相关（弃用）
        
        public string GetStudentByName(string stuName,string schoolName)
        {
            var res = "";
            List<SYS_School> schools= UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.SchoolName.Contains(schoolName) && x.Status == 1).Result;
            if(null!= schools&&schools.Count>0)
            {
                var sb = new StringBuilder();
                foreach(var item in schools)
                {
                    List<SYS_Student> stus = UnitOfWork.Repository<SYS_Student>().GetEntitiesAsync(x =>x.SchoolId==item.ID&& x.StuName.Equals(stuName) && x.Status == 1).Result;
                    if (null != stus && stus.Count > 0)
                    {
                        foreach(var s in stus)
                        {
                            SYS_Class cls= UnitOfWork.Repository<SYS_Class>().GetEntitiesAsync(x => x.SchoolId == item.ID && x.ID==s.ClassId && x.Status == 1).Result.FirstOrDefault();
                            sb.Append("<tr><td><input style='left: initial; opacity: 1; position: initial;' type='checkbox' class='stuChb' stuId="+s.ID+" schoolId="+s.SchoolId+" /></td>");
                            sb.Append("<td>"+s.StuName+"</td>");
                            sb.Append("<td>" + (cls==null?"未分班级":cls.ClassName) + "</td>");
                            sb.Append("<td>" + s.Phone.Substring(7) + "</td></tr>");
                        }
                    }
                }
                res = sb.ToString();
            }
            return res;
        }
        public int BindAndRegisterParent(string stusJson,string parentJson, ModelWxUserInfo wxUser)
        {
            var res = 0;
            try
            {
                SYS_Parent parent = JsonHelper.JsonToT<SYS_Parent>(parentJson);
                List<SYS_Student> stus = JsonHelper.JsonToT<List<SYS_Student>>(stusJson);
                parent.OpenId = wxUser.openid;
                parent.NickName = wxUser.nickname;
                parent.Status = 1;
                parent.AvatarPic = wxUser.headimgurl;
                parent.SchoolId = stus[0].SchoolId;
                parent.CreatTime = DateTime.Now;
                parent.Sex = (byte)wxUser.sex;
                UnitOfWork.Repository<SYS_Parent>().AddEntity(parent);
                var result = UnitOfWork.CommitAsync().Result;
                if (result > 0 && stus.Count > 0)
                {
                    foreach (var item in stus)
                    {
                        FK_Stu_Parent fsp = new FK_Stu_Parent()
                        {
                            StuId = item.ID,
                            ParentId = parent.ID,
                            OpenId = parent.OpenId,
                            SchoolId = item.SchoolId,
                            Enabled = true
                        };
                        UnitOfWork.Repository<FK_Stu_Parent>().AddEntity(fsp);
                    }
                    res = UnitOfWork.CommitAsync().Result;
                }
            }
            catch (Exception e)
            {
                LogHelper.Error(e.ToString());
            }
            return res; 
        }
        #endregion

        #region 职员绑定注册相关（弃用）
        public string GetStaffByInfo(string staffInfo, string schoolName)
        {
            var res = "";
            var schools = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.SchoolName.Contains(schoolName) && x.Status == 1).Result;
            if (null != schools && schools.Count > 0)
            {
                var sb = new StringBuilder();
                foreach (var item in schools)
                {
                    var staff = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.SchoolId == item.ID && (x.StaffName.Equals(staffInfo)||x.Phone==staffInfo) && x.Status == 1).Result.FirstOrDefault();
                    if (null != staff)
                    {
                        var role = UnitOfWork.Repository<SYS_StaffRole>().GetEntitiesAsync(x => x.ID == staff.RoleId).Result.FirstOrDefault();
                        sb.Append("<tr><td><input style='left: initial; opacity: 1; position: initial;' type='checkbox' checked class='stuChb' staffId=" + staff.ID + " schoolId=" + staff.SchoolId + " /></td>");
                        sb.Append("<td>" + staff.StaffName + "</td>");
                        sb.Append("<td>" + (role == null?"未选":role.RoleName) + "</td>");
                        sb.Append("<td>" + staff.Phone.Substring(7) + "</td></tr>");
                    }
                }
                res = sb.ToString();
            }
            return res;
        }
        /// <summary>
        /// 绑定或者注册
        /// </summary>
        public int BindOrRegisterStaff(int staffId,string staffJson, ModelWxUserInfo wxUser)
        {
            var res = 0;
            try
            {
                if (staffId > 0)//绑定
                {
                    var staff = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.ID == staffId && x.Status == 1).Result.FirstOrDefault();
                    staff.OpenId = wxUser.openid;
                    staff.NickName = wxUser.nickname;
                    staff.AvatarPic = wxUser.headimgurl;
                    staff.Sex = (byte)wxUser.sex;
                    UnitOfWork.Repository<SYS_Staff>().UpdateEntity(staff);
                }
                else //注册
                {
                    var staff= JsonHelper.JsonToT<SYS_Staff>(staffJson);
                    staff.CreateTime = DateTime.Now;
                    staff.OpenId = wxUser.openid;
                    staff.NickName = wxUser.nickname;
                    staff.AvatarPic = wxUser.headimgurl;
                    staff.Sex = (byte)wxUser.sex;
                    UnitOfWork.Repository<SYS_Staff>().AddEntity(staff);
                }
                res = UnitOfWork.CommitAsync().Result;

            }
            catch (Exception e)
            {
                LogHelper.Error(e.ToString());
            }
            return res;
        }
        
        #endregion

    }
}
