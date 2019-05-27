using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.IO;

namespace PinkOneSysCore.Areas.SchoolRelated.Controllers
{
    [Area("SchoolRelated")]
    [SchoolLoginFilter]
    public class StaffController : BaseController<ICrewService>
    {
        // GET: SchoolRelated/Staff
        public ActionResult Index()
        {
            return View("Index_Staff");
        }
        public ActionResult StaffDetail(int staffId)
        {
            return View("Index_StaffDetail", Service.GetStaffDetailModel(staffId));
        }

        [HttpPost]
        public JsonResult AddStaffPinkoneAccount(int staffId, string account,string password)
        {
            mjResult = Service.AddStaffPinkoneAccount(staffId, account, password);
            return Json(mjResult);
        }

        [HttpGet]
        public JsonResult GetStaffInfo(string query)
        {
            var res = Service.GetStaffInfo(query);
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
        public JsonResult AddOrModifyStaff(byte type,string entity)
        {
            var res = Service.AddOrModifyStaff(type, entity);
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
        public JsonResult GetStaffById(int id)
        {
            var res = Service.GetStaffById(id);
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
        public JsonResult DelStaffById(long id)
        {
            var res = Service.DelStaffById(id);
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

        /// <summary>
        /// 批量导入职员
        /// </summary>
        public async Task<JsonResult> ImportStaffs()
        {
            var res = "";
            var form = HttpContext.Request.Form;
            if (form.Files.Count > 0)
            {
                var file = form.Files[0];
                var tmpfiledir = AppDomain.CurrentDomain.BaseDirectory + "TempFiles\\";
                if (!Directory.Exists(tmpfiledir))
                    Directory.CreateDirectory(tmpfiledir);
                var fullpathname = tmpfiledir + file.FileName;
                //file.SaveAs(fullpathname);
                using (var stream = new FileStream(fullpathname, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                res = Service.ImportStaffs(fullpathname);
            }
            if (res.Contains("OK"))
            {
                mjResult.code = 1;
                mjResult.content = res.ToString();
            }
            else
            {
                mjResult.errMsg = res;
            }
            return Json(mjResult);
        }
    }
}