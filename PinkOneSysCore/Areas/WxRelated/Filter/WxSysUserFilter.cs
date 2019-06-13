using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utility;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PinkOneSysCore.Areas.WxRelated
{
    public class WxSysUserFilter : ActionFilterAttribute
    {
        #region 覆写函数
        /// <summary>
        /// Action执行中触发委托
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.HttpContext.Request.Cookies.TryGetValue(ComConst.Wx_ModelSysWxUser, out string value);
            if (string.IsNullOrEmpty(value))
                value = string.Empty;
            var mSysWxUser = JsonHelper.JsonToT<ModelSysWxUser>(value);
            if (mSysWxUser==null||mSysWxUser.UserType <=0)
            {
                var XRWStrs = filterContext.HttpContext.Request.Headers["X-Requested-With"].ToString();
                if (null != XRWStrs && XRWStrs.Contains("XMLHttpRequest"))
                {
                    filterContext.HttpContext.Response.WriteAsync("redirectUrl,/WxRelated/WxHome");
                }
                else
                {
                    filterContext.Result = new RedirectResult("~/WxRelated/WxHome");
                }
            }
            else
            {
                filterContext.HttpContext.Response.Cookies.Append(ComConst.Wx_ModelSysWxUser, JsonHelper.ToJson(mSysWxUser), ComHelper.GetCookieOpetion());
                filterContext.HttpContext.Session.SetString(ComConst.Wx_ModelSysWxUser, JsonHelper.ToJson(mSysWxUser));
            }
        }
        #endregion

    }
}