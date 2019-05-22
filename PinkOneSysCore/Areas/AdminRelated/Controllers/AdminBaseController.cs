using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utility;
using Microsoft.AspNetCore.Mvc;

namespace PinkOneSysCore.Areas.AdminRelated.Controllers
{
    [Area("AdminRelated")]
    public class AdminBaseController<TService> : Controller
       where TService : IBaseService
    {
        public ModelLoginUser mlUser;
        public ModelJsonRet mjResult;
        //public ModelSysWxUser mSysWxUser;
        public TService Service { get; set; }
        public AdminBaseController()
        {
            mlUser = JsonHelper.JsonToT<ModelLoginUser>(HttpContextCore.GetSession(ComConst.LoginUser));
            if (mlUser != null)
            {
                HttpContextCore.SetSession(ComConst.LoginUser, JsonHelper.ToJson(mlUser));
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