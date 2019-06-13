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
                GC.Collect();
                //实现一个HttpContext请求管道对应唯一UnitOfWork对象，继而对应唯一DbContext对象,以避免各种DbContext线程安全Bug
                //由于Http用完即销毁，UnitOfWork、DbContext对象也随之销毁。避免DbContext占用内存
                IUnitOfWork _unitOfWork = (UnitOfWork)HttpContextCore.GetItem("UnitOfWork");
                if (_unitOfWork == null)
                {
                    _unitOfWork = new UnitOfWork();
                    HttpContextCore.AddItem("UnitOfWork", _unitOfWork);
                }
                return _unitOfWork;
            }
        }

        public BaseService()
        {
            if (null == mlUser)
            {
                mlUser = JsonHelper.JsonToT<ModelLoginUser>(HttpContextCore.GetCookie(ComConst.UserLogin));
                if (mlUser == null)
                {
                    mlUser = JsonHelper.JsonToT<ModelLoginUser>(HttpContextCore.GetSession(ComConst.UserLogin));
                    if (mlUser != null)
                        HttpContextCore.SetSession(ComConst.UserLogin, JsonHelper.ToJson(mlUser));
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


            switch (entityName)
            {
                case "school":
                    {
                        res = mlUser.School;
                        break;
                    }
                case "class":
                    {
                        List<SYS_Class> list = null;//JsonHelper.JsonToT<List<SYS_Class>>(HttpContextCore.GetSession(ComConst.Session_ClassList));
                        if (null == list)
                        {
                            list = UnitOfWork.Repository<SYS_Class>().GetEntitiesAsync(x => x.SchoolId == schoolId && x.Status == 1).Result.OrderBy(x => x.Grade).ToList();
                        }
                        //HttpContextCore.SetSession(ComConst.Session_ClassList, JsonHelper.ToJson(list));
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
                        List<SYS_Staff> list = null;//JsonHelper.JsonToT<List<SYS_Staff>>(HttpContextCore.GetSession(ComConst.Session_StaffList));
                        if (null == list)
                        {
                            list = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.SchoolId == schoolId && x.Status == (byte)StaffStatus.在职).Result.OrderBy(x => x.RoleLevel).ToList();
                        }
                        //HttpContextCore.SetSession(ComConst.Session_StaffList, JsonHelper.ToJson(list));
                        res = (list == null ? new List<SYS_Staff>() : list);
                        break;
                    }
            }
            return res;
        }
        /// <summary>
        /// 刷新存储的Session
        /// </summary>
        public void RefreshEntities(string entityName)
        {
            if (null == mlUser || mlUser.School == null)
                return;
            int schoolId = mlUser.School.ID;
            switch (entityName)
            {
                case "class":
                    {
                        List<SYS_Class> list = UnitOfWork.Repository<SYS_Class>().GetEntitiesAsync(x => x.SchoolId == schoolId && x.Status == 1).Result.OrderBy(x => x.Grade).ToList();
                        HttpContextCore.SetSession(ComConst.Session_ClassList, JsonHelper.ToJson(list));
                        break;
                    }
                case "stu":
                    {
                        break;
                    }
                case "staff":
                    {
                        List<SYS_Staff> list = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.SchoolId == schoolId && x.Status == (byte)StaffStatus.在职).Result.OrderBy(x => x.RoleLevel).ToList();
                        HttpContextCore.SetSession(ComConst.Session_StaffList, JsonHelper.ToJson(list));
                        break;
                    }
            }
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
