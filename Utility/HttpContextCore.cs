using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public class HttpContextCore
    {
        private static IHttpContextAccessor _contextAccessor;
        public static Microsoft.AspNetCore.Http.HttpContext Current => _contextAccessor.HttpContext;
        public static void Configure(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
      
        /// <summary>
        /// 添加Sesssion
        /// </summary>
        public static void SetSession(string SesionStr, string Code)
        {
            Current.Session.SetString(SesionStr, Code);
        }

        /// <summary>
        /// 获取Session
        /// </summary>
        public static string GetSession(string SesionStr)
        {
            return Current.Session.GetString(SesionStr);
        }
        public static void AddItem(string key,object value)
        {
            Current.Items.Add(key, value);
        }
        public static object GetItem(string key)
        {
            Current.Items.TryGetValue(key, out object value);
            return value;
        }
    }
}
