using DataAccess;
using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataService
{
    public class BaseService:IBaseService
    {
        private static IUnitOfWork _unitOfWork;
        public ModelLoginUser mlUser { get; set; }
        public ModelWxSetting mWxSetting = JsonFileProvider.Instance.GetSettings<ModelWxSetting>();
        public ModelJsonRet mjRet { get; set; }

        /// <summary>
        /// 获取数据单元模块
        /// </summary>
        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (_unitOfWork == null)
                {
                    _unitOfWork = new UnitOfWork();
                }
                return _unitOfWork;
            }
        }

        public BaseService()
        {
            if (null == mlUser)
            {
                mlUser = JsonHelper.JsonToT<ModelLoginUser>(HttpContextCore.GetSession(ComConst.LoginUser));
                if (mlUser != null)
                    HttpContextCore.SetSession(ComConst.LoginUser, JsonHelper.ToJson(mlUser));
                else
                {
                    GC.Collect();
                    //最新登录的用户新建UnitOfWork，避免dbContext为单例模式
                    _unitOfWork = new UnitOfWork();
                }
            }
            mjRet = new ModelJsonRet
            {
                code = 0,
            };
        }

        /// <summary>
        /// 获取并设置Session中学校实体列表（entityName：'school','class','stu','staff'
        /// </summary>
        public object GetSchoolEntities(string entityName)
        {
            object res = null;
            if (null == mlUser || mlUser.School == null)
                return null;
            int schoolId = mlUser.School.ID;

            var t= Task.Run(() =>
            {
                switch (entityName)
                {
                    case "school":
                        {
                            res = mlUser.School;
                            break;
                        }
                    case "class":
                        {
                            List<SYS_Class> list = JsonHelper.JsonToT<List<SYS_Class>>(HttpContextCore.GetSession(ComConst.Session_ClassList));
                            if (null == list)
                            {
                                list = UnitOfWork.Repository<SYS_Class>().GetEntitiesAsync(x => x.SchoolId == schoolId && x.Status == 1).Result.OrderBy(x => x.Grade).ToList();
                            }
                            HttpContextCore.SetSession(ComConst.Session_ClassList, JsonHelper.ToJson(list));
                            res = (list == null ? new List<SYS_Class>() : list);
                            break;
                        }
                    case "stu":
                        {
                            List<SYS_Student> list = null;//JsonHelper.JsonToT<List<SYS_Student>>(HttpContextCore.GetSession(ComConst.Session_StuList));
                            if (null == list)
                            {
                                list = UnitOfWork.Repository<SYS_Student>().GetEntitiesAsync(x => x.SchoolId == schoolId && x.Status == (byte)StuStatus.正常).Result.OrderBy(x => x.Grade).ThenBy(x => x.ClassId).ToList();
                            }
                            //CookieSessionHelper.AddSession(ComConst.Session_StuList, JsonHelper.ToJson(list),1);//学生数据量大，不存在session中
                            res = (list == null ? new List<SYS_Student>() : list);
                            break;
                        }
                    case "staff":
                        {
                            List<SYS_Staff> list = JsonHelper.JsonToT<List<SYS_Staff>>(HttpContextCore.GetSession(ComConst.Session_StaffList));
                            if (null == list)
                            {
                                list = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.SchoolId == schoolId && x.Status == (byte)StaffStatus.在职).Result.OrderBy(x => x.RoleLevel).ToList();
                            }
                            HttpContextCore.SetSession(ComConst.Session_StaffList, JsonHelper.ToJson(list));
                            res = (list == null ? new List<SYS_Staff>() : list);
                            break;
                        }
                }
            });
            t.Wait();
            if (t.IsCompleted)
            {
                return res;
            }
            else
                return null;
        }

        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
