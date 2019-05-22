using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using Utility;

namespace PinkOneSysCore
{
    public class SchoolLoginFilter: ActionFilterAttribute
    {
        #region 覆写函数
        /// <summary>
        /// Action执行中触发委托
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ModelLoginUser mlUser = null;
            filterContext.HttpContext.Request.Cookies.TryGetValue(ComConst.LoginUser, out string value);
            if (value != null)
                mlUser = JsonHelper.JsonToT<ModelLoginUser>(value);
            if (null == mlUser || mlUser.School == null)
            {
                var XRWStrs = filterContext.HttpContext.Request.Headers["X-Requested-With"];
                if ( XRWStrs.Contains("XMLHttpRequest"))
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
                filterContext.HttpContext.Response.Cookies.Append(ComConst.LoginUser, JsonHelper.ToJson(mlUser), ComHelper.GetCookieOpetion());
                filterContext.HttpContext.Session.SetString(ComConst.LoginUser, JsonHelper.ToJson(mlUser));
            }
        }
        #endregion
    }
}