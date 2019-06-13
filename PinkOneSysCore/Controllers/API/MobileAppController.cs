using DataService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Utility;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PinkOneSysCore.Controllers.API
{
    public class MobileAppController : ControllerBase
    {
        private string baseFileDir = ConfigHelper.AppSettings("BaseFileDir");
        private string apiWebServer= ConfigHelper.AppSettings("ApiWebServer"); 
        private string fileWebServer= ConfigHelper.AppSettings("FileWebServer");
        public ModelJsonRet mjResult;
        public IMobileAppApiService Service { get; set; }
        public MobileAppController()
        {
            Service = new MobileAppApiService();
            mjResult = new ModelJsonRet()
            {
                code = 0,
                errMsg = string.Empty,
                content = string.Empty
            };
        }

        //[HttpPost, Route("api/MobileApp/OAuth")]
        //public ModelJsonRet OAuth()
        //{
        //    try
        //    {
        //        var form = HttpContext.Request.Form;
        //        var username = form["username"].ToString().Trim();
        //        var password = form["password"].ToString().Trim();
        //        var schoolId = int.Parse(form["schoolId"].ToString().Trim());
        //        return Service.OAuth(username,password, schoolId);
        //    }
        //    catch (Exception e)
        //    {
        //        mjResult.errMsg = e.Message;
        //        return mjResult;
        //    }
        //}

        [HttpGet, Route("api/MobileApp/Login")]
        public ModelJsonRet Login()
        {
            var form = HttpContext.Request.Form;
            var type = byte.Parse(form["type"].ToString());
            var username = form["username"].ToString().Trim();
            var password = form["password"].ToString().Trim();
            return Service.MobileAppLogin(type, username, password);
        }

        [HttpPost, Route("api/MobileApp/UploadAttendance")]
        public ModelJsonRet UploadAttendance()
        {
            try
            {
                var form = HttpContext.Request.Form;
                //var token = request["token"];
                var schoolId= int.Parse(form["schoolId"].ToString().Trim());
                long studentId = long.Parse(form["studentId"].ToString().Trim());
                byte attType = byte.Parse(form["attType"].ToString().Trim());
                byte attWay = byte.Parse(form["attWay"].ToString().Trim());
                var dateTime = form["dateTime"].ToString().Trim();
                var isSendMsg = byte.Parse(form["isSendMsg"].ToString().Trim());
                mjResult= Service.UploadAttFromApp(schoolId, studentId, attType, attWay, isSendMsg);

                return mjResult;
            }
            catch(Exception e)
            {
                mjResult.errMsg = e.Message;
                return mjResult;
            }
        }
       
    }
}
