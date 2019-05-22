using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataService
{
    public class SchoolMngService:BaseService,ISchoolMngService
    {
        public ModelSchoolManage GetSchoolMngModel()
        {
            var school = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID == mlUser.School.ID).Result.FirstOrDefault();
            var staffRoles = UnitOfWork.Repository<SYS_StaffRole>().GetEntitiesAsync(x => x.SchoolId == 0 || x.SchoolId == school.ID).Result;
            return new ModelSchoolManage
            {
                School = school,
                StaffRoles = staffRoles
            };
        }

        public int AddStaffRole(string roleName, byte roleLevel)
        {
            var oldRole = UnitOfWork.Repository<SYS_StaffRole>().GetEntitiesAsync(x => x.RoleName == roleName).Result.FirstOrDefault();
            if (oldRole == null)
            {
                var role = new SYS_StaffRole
                {
                    SchoolId = mlUser.School.ID,
                    RoleName = roleName,
                    RoleLevel = roleLevel
                };
                UnitOfWork.Repository<SYS_StaffRole>().AddEntity(role);
            }
            return UnitOfWork.CommitAsync().Result;
        }
        public int DelStaffRole(int id)
        {
            var role = UnitOfWork.Repository<SYS_StaffRole>().GetEntitiesAsync(x => x.ID == id).Result.FirstOrDefault();
            if (role != null)
                UnitOfWork.Repository<SYS_StaffRole>().DeleteEntity(role);
            return UnitOfWork.CommitAsync().Result;
        }

        public int ModifySchoolInfo(string schoolName, string contact, string address, string newPwd)
        {
            var school = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID == mlUser.School.ID).Result.FirstOrDefault();
            school.SchoolName = schoolName;
            school.ContactInfo = contact;
            school.Address = address;
            if (newPwd.Length >= 6)
            {
                school.Password = newPwd;
            }
            UnitOfWork.Repository<SYS_School>().UpdateEntity(school);
            var isOk=UnitOfWork.CommitAsync().Result;
            if (isOk > 0)
            {
                mlUser.School = school;
                //更新cookie
                //CookieSessionHelper.AddCookie(ComConst.Cookie_LoginUser, JsonHelper.ToJson(mlUser));
            }
            return isOk;
        }

        /// <summary>
        /// 更新头像数据，数据太大，无法写入cookie
        /// </summary>
        public int UploadImgData(string avatarPic)
        {
            var school = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID == mlUser.School.ID).Result.FirstOrDefault();
            school.AvatarPic = avatarPic;
            school.CityId = null;
            UnitOfWork.Repository<SYS_School>().UpdateEntity(school);
            var isOk = UnitOfWork.CommitAsync().Result;
            //if (isOk > 0)
            //{
            //    mlUser.School = school;
            //    //更新cookie
            //    CookieSessionHelper.AddCookie(ComConst.Cookie_LoginUser, JsonHelper.ToJson(mlUser));
            //}
            return isOk;
        }

        #region 首页用
        /// <summary>
        /// 获取登录用户信息
        /// </summary>
        public string GetUserInfo(int id)
        {

            var school = mlUser.School;//UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID == id && x.Status == (byte)SchoolStatus.正常).Result.FirstOrDefault();
            var notices = UnitOfWork.Repository<SYS_Notice>().GetEntitiesAsync(x => x.SchoolId == id && (x.Type == (byte)NoticeType.陪绮发给学校 || x.Type == (byte)NoticeType.学校内部)
             && x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now).Result.OrderBy(x => x.NoticeLevel).ToList();

            var json = new
            {
                school = school,
                notices = notices
            };
            return JsonHelper.ToJson(json);
        }

        /// <summary>
        /// 获取校园概览数据
        /// </summary>
        public string GetSchoolData()
        {
            var res = "";
            List<SYS_Class> classes = (List<SYS_Class>)GetSchoolEntities("class");
            List<SYS_Staff> staffs = (List<SYS_Staff>)GetSchoolEntities("staff");
            List<SYS_Student> allStus = (List<SYS_Student>)GetSchoolEntities("stu");
            if (classes.Count > 0 || staffs.Count > 0 || allStus.Count > 0)
            {
                var json = new
                {
                    stuCount = allStus.Count,
                    classCount = classes.Count,
                    staffCount = staffs.Count,
                };
                res = JsonHelper.ToJson(json);
            }
            return res;
        }
        #endregion
    }
}
