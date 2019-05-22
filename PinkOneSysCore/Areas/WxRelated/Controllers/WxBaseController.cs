using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace PinkOneSysCore.Areas.WxRelated.Controllers
{
    [Area("WxRelated")]
    public class WxBaseController<TService> : Controller
       where TService : IBaseService
    {
        public static int glbSchoolId = 0;
        public ModelJsonRet mjResult;
        public ModelSysWxUser mSysWxUser;
        public TService Service { get; set; }
        public WxBaseController()
        {
            ViewBag.SchoolName = "陪绮在线";
            mjResult = new ModelJsonRet()
            {
                code = 0,
                errMsg = string.Empty,
                content = string.Empty
            };

            mSysWxUser = JsonHelper.JsonToT<ModelSysWxUser>(HttpContextCore.GetSession(ComConst.Wx_ModelSysWxUser));
            if (mSysWxUser != null)
            {
                HttpContextCore.SetSession(ComConst.Wx_ModelSysWxUser, JsonHelper.ToJson(mSysWxUser));
                if (mSysWxUser.School != null)
                    ViewBag.SchoolName = mSysWxUser.School.SchoolName;
            }
        }

        /// <summary>
        /// 设置本地cookie
        /// </summary>
        /// <param name="minutes">过期时长，单位：分钟</param>      
        public void SetCookie(string key, string value, int minutes = 0)
        {
            HttpContext.Response.Cookies.Append(key, value, ComHelper.GetCookieOpetion());
        }
        /// <summary>
        /// 删除指定的cookie
        /// </summary>
        public void DeleteCookie(string key)
        {
            HttpContext.Response.Cookies.Delete(key);
        }

        /// <summary>
        /// 获取cookie的value
        /// </summary>
        public string GetCookie(string key)
        {
            HttpContext.Request.Cookies.TryGetValue(key, out string value);
            if (string.IsNullOrEmpty(value))
                value = string.Empty;
            return value;
        }

        /// <summary>
        /// 添加Sesssion
        /// </summary>
        public void SetSession(string SesionStr, string Code)
        {
            HttpContext.Session.SetString(SesionStr, Code);
        }

        /// <summary>
        /// 获取Session
        /// </summary>
        public string GetSession(string SesionStr)
        {
            return HttpContext.Session.GetString(SesionStr);
        }
    }
}