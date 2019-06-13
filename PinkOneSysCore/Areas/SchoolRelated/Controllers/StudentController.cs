using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

namespace PinkOneSysCore.Areas.SchoolRelated.Controllers
{
    [Area("SchoolRelated")]
    [SchoolLoginFilter]
    public class StudentController : BaseController<ICrewService>
    {
        // GET: SchoolRelated/Student
        public ActionResult Index()
        {
            return View("Index_Student");
        }
        //public action

        public ActionResult StuDetail(long stuId)
        {
            return View("Index_StuDetail", Service.GetStuDetailModel(stuId));
        }


        [HttpGet]
        public JsonResult GetStuInfo(string nameQuery,int classQuery,int pageIndex)
        {
            var res = Service.GetStuInfo(nameQuery, classQuery, pageIndex);
            if (res.Length > 6)
            {
                mjResult.code = 1;
                mjResult.content = res;
            }
            else
            {
                mjResult.errMsg = "您还没有学生，请添加";
            }
            return Json(mjResult);
        }
        [HttpPost]
        public JsonResult AddOrModifyStu(byte type, string entity)
        {
            var res = Service.AddOrModifyStu(type, entity);
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
        public JsonResult GetStuById(int id)
        {
            var res = Service.GetStuById(id);
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
        public JsonResult DelStuById(long id)
        {
            var res = Service.DelStuById(id);
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
        /// 批量导入学生
        /// </summary>
        public async Task<JsonResult> ImportStus(int classId)
        {
            var res = "";
            var form = HttpContext.Request.Form;
            if (form.Files.Count > 0)
            {
                var file = form.Files[0];
                var tmpfiledir = AppDomain.CurrentDomain.BaseDirectory + "TempFiles\\";
                if (!Directory.Exists(tmpfiledir))
                    Directory.CreateDirectory(tmpfiledir);
                var fullpathname= tmpfiledir+ file.FileName;
                //file.SaveAs(fullpathname);
                using (var stream = new FileStream(fullpathname, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
                res =  Service.ImportStus(classId, fullpathname);
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

        public JsonResult OperateCard(byte type,long cardId)
        {
            mjResult = Service.OperateCard(type, cardId);
            return Json(mjResult);
        }
    }
}