using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utility;
using Microsoft.AspNetCore.Mvc;
using Domain;

namespace PinkOneSysCore.Areas.AdminRelated.Controllers
{
    [Area("AdminRelated")]
    public class AdminBaseController<TService> : Controller
       where TService : IBaseService
    {
        public SYS_Admin glbAdmin;
        public ModelJsonRet mjResult;
        public TService Service { get; set; }
        public AdminBaseController()
        {
            glbAdmin = JsonHelper.JsonToT<SYS_Admin>(HttpContextCore.GetSession(ComConst.AdminLogin));
            if (glbAdmin != null)
            {
                HttpContextCore.SetSession(ComConst.AdminLogin, JsonHelper.ToJson(glbAdmin));
            }
            mjResult = new ModelJsonRet()
            {
                code = 0,
                errMsg = string.Empty,
                content = string.Empty
            };

        }
    }
}