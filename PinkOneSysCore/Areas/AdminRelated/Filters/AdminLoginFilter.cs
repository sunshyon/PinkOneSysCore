using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace PinkOneSysCore.Areas.AdminRelated
{
    public class AdminLoginFilter: ActionFilterAttribute
    {
        #region 覆写函数
        /// <summary>
        /// Action执行中触发委托
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Domain.SYS_Admin admin=null;
            filterContext.HttpContext.Request.Cookies.TryGetValue(ComConst.AdminLogin, out string value);
            if(value!=null)
                admin = JsonHelper.JsonToT<Domain.SYS_Admin>(value);
            if (null == admin )
            {
                var a = filterContext.HttpContext.Request.Headers;
                var XRWStrs = filterContext.HttpContext.Request.Headers["X-Requested-With"].ToString();
                if (null != XRWStrs && XRWStrs.Contains("XMLHttpRequest"))
                {
                    filterContext.HttpContext.Response.WriteAsync("redirectUrl,/Login");
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/Login");
                }
            }
            else
            {
                filterContext.HttpContext.Response.Cookies.Append(ComConst.AdminLogin, JsonHelper.ToJson(admin), ComHelper.GetCookieOpetion());
                filterContext.HttpContext.Session.SetString(ComConst.AdminLogin, JsonHelper.ToJson(admin));
            }
        }

        #endregion
    }
}