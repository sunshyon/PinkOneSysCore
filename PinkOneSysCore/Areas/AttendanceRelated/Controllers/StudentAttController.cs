using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace PinkOneSysCore.Areas.AttendanceRelated.Controllers
{
    [Area("AttendanceRelated")]
    [SchoolLoginFilter]
    public class StudentAttController : BaseController<IAttendanceService>
    {

        public ActionResult StuAttNow()
        {
            return View("StuAttNow");
        }
        public ActionResult StuAttNowById(int classId)
        {
            ViewBag.ClassId = classId;
            return View("StuAttNow");
        }
        public ActionResult StuAttDetail()
        {
            return View("StuAttDetail");
        }
        public ActionResult StuAttDetailByName(string stuName)
        {
            ViewBag.StuName = stuName;
            return View("StuAttDetail");
        }
        public ActionResult StuAttMouth()
        {
            return View("StuAttMouth");
        }

        public JsonResult GetClassesAttNow()
        {
            var res = Service.GetClassesAttNow();
            if (res.Length > 6)
            {
                mjResult.code = 1;
                mjResult.content = res;
            }
            else
            {
                mjResult.errMsg = "未找到考勤记录";
            }
            return Json(mjResult);
        }
        public JsonResult GetStuAttNow(string nameQuery,int classQuery)
        {
            var res = Service.GetStuAttNow(nameQuery,classQuery);
            if (res.Length > 6)
            {
                mjResult.code = 1;
                mjResult.content = res;
            }
            else
            {
                mjResult.errMsg = "未找到考勤记录";
            }
            return Json(mjResult);
        }
        public JsonResult AddStuAttManually(long stuId,byte attType,string attTime,string attTemp,string attRemark)
        {
            var res = Service.AddStuAttManually(stuId, attType,attTime, attTemp,attRemark);
            if (res > 0)
            {
                mjResult.code = 1;
                mjResult.content = "ok";
            }
            else
            {
                mjResult.errMsg = "未找到学生";
            }
            return Json(mjResult);
        }
        
        public JsonResult GetStuAttDetail(string nameQuery, int classQuery, string sTime, string eTime)
        {
            var res = Service.GetStuAttDetails(nameQuery, classQuery, sTime,eTime);
            if (res.Length > 6)
            {
                mjResult.code = 1;
                mjResult.content = res;
            }
            else
            {
                mjResult.errMsg = "未找到考勤记录";
            }
            return Json(mjResult);
        }
        public JsonResult GetStuAttMouth(int classQuery, string attMouth)
        {
            var res = Service.GetStuAttMouth( classQuery, attMouth);
            if (res.Length > 6)
            {
                mjResult.code = 1;
                mjResult.content = res;
            }
            else
            {
                mjResult.errMsg = "未找到考勤记录";
            }
            return Json(mjResult);
        }
    }
}