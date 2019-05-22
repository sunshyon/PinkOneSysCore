using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataService
{
    public class AdminService: BaseService,IAdminService
    {
        /// <summary>
        /// 获取系统概要数据
        /// </summary>
        /// <returns></returns>
        public ModelSchoolsSummary GetSchoolsSummerModel()
        {
            var mss = new ModelSchoolsSummary
            {
                TotalSchool = 0,
                TotalAllStudent = 0,
                TotalOnSchoolStudent = 0,
                TotalStaff = 0,
                TotalAllCard = 0,
                TotalUseCard = 0,
            };
            var tSchool = (int)UnitOfWork.Repository<SYS_School>().CountEntityAsync(x => x.Status == (byte)SchoolStatus.正常).Result;
            var tAllStu= UnitOfWork.Repository<SYS_Student>().CountEntityAsync(x => true).Result;
            var tOnSchoolStu = UnitOfWork.Repository<SYS_Student>().CountEntityAsync(x=>x.Status==(byte)StuStatus.正常).Result;
            var tStaff = (int)UnitOfWork.Repository<SYS_Staff>().CountEntityAsync(x => x.Status == (byte)StaffStatus.在职).Result;
            var tAllCard = UnitOfWork.Repository<SYS_Card>().GetEntitiesAsync(x => true).Result.LongCount();
            //var tUseCard = UnitOfWork.ExecuteSqlQuery<long>("select count(*) from SYS_Card where Status").FirstOrDefault();
            mss.TotalSchool = tSchool;
            mss.TotalAllStudent = tAllStu;
            mss.TotalOnSchoolStudent = tOnSchoolStu;
            mss.TotalStaff = tStaff;
            mss.TotalUseCard = tAllCard;
            return mss;
        }

        /// <summary>
        /// 获取所有学校信息
        /// </summary>
        public string GetSchoolsInfo(string query)
        {
            var res = "<tr><td colspan='7' style='color:red;'>无学校</td></tr>";
            var schools = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x=>true).Result;
            if (null != query && query.Length > 0)
                schools = schools.Where(x => x.SchoolName.Contains(query) || x.Username.Equals(query)).ToList();

            if (null != schools && schools.Count > 0)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var item in schools)
                {
                    sb.Append("<tr>");
                    sb.Append("<td>" + item.SchoolName + "</td>");
                    sb.Append("<td>" + item.Username + "</td>");
                    sb.Append("<td>" + (item.Type==null?"未选":((SchoolType)item.Type).ToString()) + "</td>");
                    sb.Append("<td>" + (SchoolStatus)item.Status+"</td>");
                    sb.Append("<td>" + item.ContactInfo + "</td>");
                    sb.Append("<td>" + item.Address + "</td>");
                    string operateStr = "<button class='btn btn-primary btn-xs' onclick='modifySchool(" + item.ID + ")'><i class='fa fa-pencil'></i></button>" +
                     "<button class='btn btn-danger btn-xs' onclick='delSchool(" + item.ID + ")'><i class='fa fa-trash'></i></button>"+
                     "<button class='btn btn-info btn-xs' onclick='openSchoolDetailPage(" + item.ID + ")'> 详情</button></td></tr> ";
                    sb.Append("<td>" + operateStr + "</td>");
                    sb.Append("</tr>");
                }
                res = sb.ToString();
            }
            return res;
        }
        
        /// <summary>
        /// 新增或修改实体(type->1:新增,2:修改)
        /// </summary>
        public string AddOrModifySchool(byte type, string entity)
        {
            var res = "";
            if (entity.Length > 6)
            {
                var obj = JsonHelper.JsonToT<SYS_School>(entity);
                if (null != obj)
                {
                    if (type == 1)
                    {
                        obj.CreateTime = DateTime.Now;

                        var oldAdmin= UnitOfWork.Repository<SYS_Admin>().GetEntitiesAsync(x => x.Username == obj.Username).Result.FirstOrDefault();
                        var oldSchool = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.Username == obj.Username).Result.FirstOrDefault();
                        var isOk = 0;
                        //学校账号和管理员账号也不能重复
                        if (oldSchool == null&& oldAdmin==null)
                        {
                            UnitOfWork.Repository<SYS_School>().AddEntity(obj);
                            isOk = UnitOfWork.CommitAsync().Result;
                            if (isOk > 0)
                                res = "OK,添加成功";
                        }
                        else
                        {
                            res = "账号重复，无法添加";
                        }

                    }
                    //修改职员
                    else
                    {
                        UnitOfWork.Repository<SYS_School>().UpdateEntity(obj);
                        var isOk= UnitOfWork.CommitAsync().Result;
                        if (isOk > 0)
                            res = "OK,修改成功";
                    }
                }
            }
            return res;
        }
        
        /// <summary>
        /// 通过ID获取Staff
        /// </summary>
        public string GetSchoolById(int id)
        {
            var res = "";
            var entity = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID == id).Result.FirstOrDefault();
            if (null != entity)
            {
                res = JsonHelper.ToJson(entity);
            }
            return res;
        }
        /// <summary>
        /// 通过Id删除实体
        /// </summary>
        public int DelSchoolById(long id)
        {
            var res = 0;
            var obj = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x=>x.ID==id).Result.FirstOrDefault();
            if (null != obj)
            {
                UnitOfWork.Repository<SYS_School>().DeleteEntity(obj);
                res = UnitOfWork.CommitAsync().Result;
            }
            return res;
        }
        public ModelSchoolDetail GetSchoolDetailModel(int id)
        {
            var msd = new ModelSchoolDetail()
            {
                TotalStudent = 0,
                TotalStaff = 0,
                TotalClass = 0,
                TotalCard = 0
            };
            var school = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID == id).Result.FirstOrDefault();
            if (school != null)
            {
                var totalStu = UnitOfWork.Repository<SYS_Student>().CountEntityAsync(x => x.SchoolId == id).Result;
                var totalSaff= (int)UnitOfWork.Repository<SYS_Staff>().CountEntityAsync(x => x.SchoolId == id).Result; 
                var totalClass = (int)UnitOfWork.Repository<SYS_Class>().CountEntityAsync(x => x.SchoolId == id).Result; 
                var totalCard = UnitOfWork.Repository<SYS_Card>().CountEntityAsync(x => x.SchoolId == id).Result;

                msd.School = school;
                msd.TotalStudent = totalStu;
                msd.TotalStaff = totalSaff;
                msd.TotalClass = totalClass;
                msd.TotalCard = totalCard;
            }
            return msd;
        }

        /// <summary>
        /// 获取管理员设置数据
        /// </summary>
        /// <param name="type">管理员类型1：超级管理员，2：普通管理员</param>
        public string GetAdminSettingInfo(byte type)
        {
            var baseStaffRoles = UnitOfWork.Repository<SYS_StaffRole>().GetEntitiesAsync(x => x.SchoolId == 0).Result.ToList();
            var sb_roles = new StringBuilder("<span  class='text-light-red'>无通用角色，请添加</span>");
            if (baseStaffRoles.Count > 0)
            {
                sb_roles = new StringBuilder();
                foreach(var role in baseStaffRoles)
                {
                    sb_roles.Append("<span title='点击删除' style='cursor: pointer; ' onclick='delStaffRole("+role.ID+")' class='text-light-blue'>&ensp;"+role.RoleName+"&ensp;</span>");
                }
            }
            var sb_admin = new StringBuilder(); ;
            if (type == 1)
            {
                var admins = UnitOfWork.Repository<SYS_Admin>().GetEntitiesAsync(x => x.Type > 1).Result;
                sb_admin = new StringBuilder("<tr><td colspan='5' style='color:red;'>无管理员账号</td></tr>");
                if (admins.Count > 0)
                {
                    sb_admin = new StringBuilder();
                    foreach (var item in admins)
                    {
                        sb_admin.Append("<tr><td>" + item.Username + "</td>");
                        sb_admin.Append("<td>" + item.Password + "</td>");
                        sb_admin.Append("<td>" + item.PersonName + "</td>");
                        sb_admin.Append("<td>" + item.Phone + "</td>");
                        sb_admin.Append("<td><button class='btn btn-danger btn-xs' onclick='delAdmin(" + item.ID + ")'><i class='fa fa-trash'></i></button></td></tr>");
                    }
                }
            }
            var json = new
            {
                baseStaffRoles = sb_roles.ToString(),
                admins = sb_admin.ToString()
            };
            return JsonHelper.ToJson(json);
        }

        public int AddStaffBaseRole(string roleName, byte roleLevel)
        {
            var oldRole = UnitOfWork.Repository<SYS_StaffRole>().GetEntitiesAsync(x => x.RoleName == roleName&&x.SchoolId==0).Result.FirstOrDefault();
            if (oldRole == null)
            {
                var role = new SYS_StaffRole
                {
                    SchoolId = 0,
                    RoleName = roleName,
                    RoleLevel = roleLevel
                };
                UnitOfWork.Repository<SYS_StaffRole>().AddEntity(role);
            }
            return UnitOfWork.CommitAsync().Result;
        }
        public int DelStaffBaseRole(int id)
        {
            var role = UnitOfWork.Repository<SYS_StaffRole>().GetEntitiesAsync(x => x.ID == id).Result.FirstOrDefault();
            if (role != null)
                UnitOfWork.Repository<SYS_StaffRole>().DeleteEntity(role);
            return UnitOfWork.CommitAsync().Result;
        }

        public string AddNewAdmin(string username, string password, string personName, string phone)
        {
            var res = "账号重复,无法添加！";
            var oldAdmin = UnitOfWork.Repository<SYS_Admin>().GetEntitiesAsync(x => x.Username == username).Result.FirstOrDefault();
            if (oldAdmin == null)
            {
                var admin = new SYS_Admin
                {
                    Username = username,
                    Password = password,
                    PersonName = personName,
                    Phone = phone,
                    CreateTime = DateTime.Now,
                    Type = 2
                };
                UnitOfWork.Repository<SYS_Admin>().AddEntity(admin);
                if (UnitOfWork.CommitAsync().Result > 0)
                    res = "OK,添加成功";
            }
            return res;
        }
        public int DelAdmin(int id)
        {
            var res = 0;
            var admin = UnitOfWork.Repository<SYS_Admin>().GetEntitiesAsync(x => x.ID == id).Result.FirstOrDefault();
            if (admin != null)
            {
                UnitOfWork.Repository<SYS_Admin>().DeleteEntity(admin);
                res=UnitOfWork.CommitAsync().Result;
            }
            return res;
        }
    }
}
