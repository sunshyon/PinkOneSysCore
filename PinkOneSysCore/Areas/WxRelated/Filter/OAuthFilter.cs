using DataService;
using Domain;
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
    public class OAuthFilter: ActionFilterAttribute
    {
        #region 覆写函数
        /// <summary>
        /// Action执行中触发委托
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string code = filterContext.HttpContext.Request.Query["code"].ToString();
            var wxPubInfo = MemoryCacheHelper.GetCache<Wx_PublicInfo>("WxPubInfo");
    
            if (null != code && code.Length > 0)
            {
                ModelWxUserInfo mWxUserInfo = WXOAuthApiHelper.GetUserInfo(wxPubInfo.AppId, wxPubInfo.AppSecret, code);
                if (mWxUserInfo != null)
                {
                    filterContext.HttpContext.Response.Cookies.Append(ComConst.Wx_ModelWxUserInfo, JsonHelper.ToJson(mWxUserInfo), ComHelper.GetCookieOpetion());
                    filterContext.HttpContext.Session.SetString(ComConst.Wx_ModelWxUserInfo, JsonHelper.ToJson(mWxUserInfo));
                }
            }
            else
            {
                if (filterContext.HttpContext.Request.Cookies.TryGetValue(ComConst.Wx_ModelWxUserInfo, out string value))
                {
                    filterContext.HttpContext.Response.Cookies.Append(ComConst.Wx_ModelWxUserInfo, value, ComHelper.GetCookieOpetion());
                    filterContext.HttpContext.Session.SetString(ComConst.Wx_ModelWxUserInfo, value);
                }
                else
                {
                    var rst = new ContentResult();
                    rst.Content = "登录过期，请退出重新进入";
                    filterContext.Result = rst;
                }
            }
        }
        #endregion

    }

}