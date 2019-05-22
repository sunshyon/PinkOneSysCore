using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utility;

namespace PinkOneSysCore
{
    public static class ComHelper
    {
        static int minutes =int.Parse( ConfigHelper.AppSettings("LoginTimeout"));
        public static CookieOptions GetCookieOpetion(int timeout=0)
        {
            if (timeout > 0)
                minutes = timeout;
            var co = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.Now.AddMinutes(minutes),
                IsEssential = true
            };
            return co;
        }
    }
}
