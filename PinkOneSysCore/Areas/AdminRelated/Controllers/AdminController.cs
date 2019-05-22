using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace PinkOneSysCore.Areas.AdminRelated.Controllers
{
    [AdminLoginFilter]
    public class AdminController : AdminBaseController<IAdminService>
    {
        // GET: AdminRelated/Admin
        public ActionResult Index()
        {
            return View("Index_Admin",Service.GetSchoolsSummerModel());
        }
        /// <summary>
        /// 学校管理页
        /// </summary>
        public ActionResult AdminMng()
        {
            return View("Index_AdminMng");
        }
        /// <summary>
        /// 学校管理页
        /// </summary>
        public ActionResult AdminSetting()
        {
            return View("Index_AdminSetting");
        }
        public ActionResult SchoolDetail(int id)
        {
            return View("Index_SchoolDetail",Service.GetSchoolDetailModel(id));
        }


        /// <summary>
        /// 获取登录信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAdminInfo()
        {
            mjResult.code = 1;
            mjResult.content =  mlUser.Admin.Username;

            return Json(mjResult);
        }

        [HttpGet]
        public JsonResult GetSchoolsInfo(string query)
        {
            var res = Service.GetSchoolsInfo(query);
            if (res.Length > 6)
            {
                mjResult.code = 1;
                mjResult.content = res;
            }
            else
            {
                mjResult.errMsg = "您还没有职员，请添加";
            }
            return Json(mjResult);
        }
        [HttpPost]
        public JsonResult AddOrModifySchool(byte type, string entity)
        {
            var res = Service.AddOrModifySchool(type, entity);
            if (res.Contains("OK"))
            {
                mjResult.code = 1;
                mjResult.content = "操作完成";
            }
            else
            {
                mjResult.errMsg = res;
            }
            return Json(mjResult);
        }
        [HttpGet]
        public JsonResult GetSchoolById(int id)
        {
            var res = Service.GetSchoolById(id);
            if (res.Length > 6)
            {
                mjResult.code = 1;
                mjResult.content = res;
            }
            else
            {
                mjResult.errMsg = "网络开小差了，稍后再试";
            }
            return Json(mjResult);
        }
        [HttpGet]
        public JsonResult DelSchoolById(long id)
        {
            var res = Service.DelSchoolById(id);
            if (res > 0)
            {
                mjResult.code = 1;
                mjResult.content = res.ToString();
            }
            else
            {
                mjResult.errMsg = "网络开小差了，稍后再试";
            }
            return Json(mjResult);
        }

        [HttpGet]
        public JsonResult GetAdminSettingInfo()
        {
            byte type = 2;
            if (mlUser.Admin != null)
                type = (byte)mlUser.Admin.Type;
            var res = Service.GetAdminSettingInfo(type);
            if (res.Length > 6)
            {
                mjResult.code = 1;
                mjResult.content = res;
            }
            else
            {
                mjResult.errMsg = "发生错误";
            }
            return Json(mjResult);
        }

        public JsonResult AddStaffBaseRole(string roleName, byte roleLevel)
        {
            var res = 0;
            res = Service.AddStaffBaseRole(roleName, roleLevel);
            if (res > 0)
            {
                mjResult.code = 1;
                mjResult.content = "ok";
            }
            else
            {
                mjResult.errMsg = "添加失败,可能原因角色重复";
            }
            return Json(mjResult);
        }
        public JsonResult DelStaffBaseRole(int id)
        {
            var res = 0;
            res = Service.DelStaffBaseRole(id);
            if (res > 0)
            {
                mjResult.code = 1;
                mjResult.content = "ok";
            }
            else
            {
                mjResult.errMsg = "删除失败";
            }
            return Json(mjResult);
        }
        [HttpPost]
        public JsonResult AddNewAdmin(string username,string password,string personName,string phone)
        {
            var res = "";
            res = Service.AddNewAdmin(username, password,personName,phone);
            if (res.Contains("OK"))
            {
                mjResult.code = 1;
                mjResult.content = "ok";
            }
            else
            {
                mjResult.errMsg = res;
            }
            return Json(mjResult);
        }
        [HttpPost]
        public JsonResult DelAdmin(int id)
        {
            var res = 0;
            res = Service.DelAdmin(id);
            if (res > 0)
            {
                mjResult.code = 1;
                mjResult.content = "ok";
            }
            else
            {
                mjResult.errMsg = "删除失败";
            }
            return Json(mjResult);
        }

        //public 
    }
}