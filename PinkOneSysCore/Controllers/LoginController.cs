using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PinkOneSysCore.Controllers
{
    public class LoginController  :BaseController<ILoginService>
    {
        // GET: Login
        public ActionResult Index()
        {
            return View("Index_Login");
        }
        public ActionResult Admin()
        {
            return View("Index_AdminLogin");
        }

        public ActionResult Verify(string name, string pwd)
        {
            var user = Service.GetUserLoginInfo(name, pwd);
            if (user.School!=null)
            {
                mlUser = user;

                SetSession(Utility.ComConst.UserLogin, Utility.JsonHelper.ToJson(user));
                SetCookie(Utility.ComConst.UserLogin, Utility.JsonHelper.ToJson(user));
                var res = 1;//学校账号
                if (user.Staff != null)
                    res = 2;//老师账号
                mjResult.code = 1;
                mjResult.content = res;
            }
            return Json(mjResult);
        }
        public ActionResult AdminVerify(string name, string pwd)
        {
            var admin = Service.GetAdminLoginInfo(name, pwd);
            if (admin != null)
            {
                SetSession(Utility.ComConst.AdminLogin, Utility.JsonHelper.ToJson(admin));
                SetCookie(Utility.ComConst.AdminLogin, Utility.JsonHelper.ToJson(admin));
               
                mjResult.code = 1;
                mjResult.content = "OK";
            }
            return Json(mjResult);
        }
        public ActionResult Logout()
        {
            DeleteCookie(Utility.ComConst.UserLogin);
            return Redirect("/Login");
        }
        public ActionResult AdminLogout()
        {
            DeleteCookie(Utility.ComConst.AdminLogin);
            return Redirect("/Login/Admin");
        }
    }
}