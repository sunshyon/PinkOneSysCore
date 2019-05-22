using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Utility;
using DataService;

namespace PinkOneSysCore
{
    public class BaseController<TService> : Controller
        where TService : IBaseService
    {
        public ModelLoginUser mlUser;
        public ModelJsonRet mjResult;
        public TService Service { get; set; }

        public BaseController()
        {

            mlUser = JsonHelper.JsonToT<ModelLoginUser>(HttpContextCore.GetSession(ComConst.LoginUser));
            if (mlUser != null)
                HttpContextCore.SetSession(ComConst.LoginUser, JsonHelper.ToJson(mlUser));
            mjResult = new ModelJsonRet()
            {
                code = 0,
                errMsg = string.Empty,
                content = string.Empty
            };
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
