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

        public ActionResult Verify(string name, string pwd)
        {
            var user = Service.GetLoginInfo(name, pwd);
            if (user.School!=null|| user.Admin!=null)
            {
                mlUser = user;

                SetSession(Utility.ComConst.LoginUser, Utility.JsonHelper.ToJson(user));
                SetCookie(Utility.ComConst.LoginUser, Utility.JsonHelper.ToJson(user));
                var res = 1;
                if (user.Admin == null)
                    res = 2;
                mjResult.code = 1;
                mjResult.content = res;
            }
            return Json(mjResult);
        }
        public ActionResult Logout()
        {
            DeleteCookie(Utility.ComConst.LoginUser);
            return Redirect("/Login");
        }

    }
}