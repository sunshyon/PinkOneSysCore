using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace PinkOneSysCore.Areas.AttendanceRelated.Controllers
{
    [SchoolLoginFilter]
    [Area("AttendanceRelated")]
    public class StaffAttController : BaseController<IAttendanceService>
    {
        public ActionResult StaffAttNow()
        {
            return View("StaffAttNow");
        }
        public ActionResult StaffAttDetail()
        {
            return View("StaffAttDetail");
        }
        public ActionResult StaffAttDetailByName(string staffName)
        {
            ViewBag.StaffName = staffName;
            return View("StaffAttDetail");
        }
        public ActionResult StaffAttMouth()
        {
            return View("StaffAttMouth");
        }

        public JsonResult GetStaffAttNow(string nameQuery)
        {
            var res = Service.GetStaffAttNow(nameQuery);
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
        public JsonResult AddStaffAttManually(int staffId, byte attType, string attTime, string attRemark)
        {
            var res = Service.AddStaffAttManually(staffId, attType, attTime, attRemark);
            if (res > 0)
            {
                mjResult.code = 1;
                mjResult.content = "ok";
            }
            else
            {
                mjResult.errMsg = "未找到职员";
            }
            return Json(mjResult);
        }

        public JsonResult GetStaffAttDetail(string nameQuery,string sTime, string eTime)
        {
            var res = Service.GetStaffAttDetails(nameQuery, sTime, eTime);
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
        public JsonResult GetStaffAttMouth( string attMouth)
        {
            var res = Service.GetStaffAttMouth(attMouth);
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