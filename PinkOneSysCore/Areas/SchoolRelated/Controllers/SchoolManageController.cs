using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace PinkOneSysCore.Areas.SchoolRelated.Controllers
{
    [Area("SchoolRelated")]
    [SchoolLoginFilter]
    public class SchoolManageController : BaseController<ISchoolMngService>
    {
        // GET: SchoolRelated/SchoolManage
        public ActionResult Index()
        {
            return View("Index_SchoolManage", Service.GetSchoolMngModel());
        }

        public JsonResult AddStaffRole(string roleName,byte roleLevel)
        {
            var res = 0;
            res=Service.AddStaffRole(roleName, roleLevel);
            if (res > 0)
            {
                mjResult.code = 1;
                mjResult.content = "ok";
            }
            else
            {
                mjResult.errMsg = "添加失败，可能原因角色重复";
            }
            return Json(mjResult);
        }
        public JsonResult DelStaffRole(int id)
        {
            var res = 0;
            res = Service.DelStaffRole(id);
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
        public JsonResult ModifySchoolInfo(string schoolName, string contact, string address,string newPwd)
        {
            var res = 0;
            res = Service.ModifySchoolInfo(schoolName, contact,address,newPwd);
            if (res > 0)
            {
                mjResult.code = 1;
                mjResult.content = "ok";
            }
            else
            {
                mjResult.errMsg = "添加失败";
            }
            return Json(mjResult);
        }
        public JsonResult UploadImgData(string avatarPic)
        {
            var res = 0;
            res = Service.UploadImgData(avatarPic);
            if (res > 0)
            {
                mjResult.code = 1;
                mjResult.content = "ok";
            }
            else
            {
                mjResult.errMsg = "添加失败";
            }
            return Json(mjResult);
        }
    }
}