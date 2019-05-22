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
    public class ClassController : BaseController<ICrewService>
    {
        // GET: SchoolRelated/Class
        public ActionResult Index()
        {
            return View("Index_Class");
        }
        [HttpGet]
        public ActionResult GetClassInfo()
        {
            var res = Service.GetClassInfo();
            if (res.Length > 6)
            {
                mjResult.code = 1;
                mjResult.content = res;
            }
            else
            {
                mjResult.errMsg = "请添加班级";
            }
            return Json(mjResult);
        }
        [HttpPost]
        public ActionResult AddOrModifyClass(byte type, string entity)
        {
            var res = Service.AddOrModifyClass(type, entity);
            if (res > 0)
            {
                mjResult.code = 1;
                mjResult.content = "操作成功";
            }
            else
            {
                mjResult.errMsg = "操作失败";
            }
            return Json(mjResult);
        }
        [HttpGet]
        public ActionResult GetClassById(int id)
        {
            var res = Service.GetClassById(id);
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
        public ActionResult DelClassById(long id)
        {
            var res = Service.DelClassById(id);
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
    }
}