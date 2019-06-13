using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Utility
{
    public class ErrorHandleMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandleMiddleware(RequestDelegate next)
        {
            this._next = next;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var statusCode = context.Response.StatusCode;
                if (ex is ArgumentException)
                {
                    statusCode = 200;
                }
                await HandleExceptionAsync(context, statusCode, ex);
            }
            //finally
            //{
            //    var statusCode = context.Response.StatusCode;
            //    var msg = "";
            //    if (statusCode == 401)
            //    {
            //        msg = "未授权";
            //    }
            //    else if (statusCode == 404)
            //    {
            //        msg = "未找到服务";
            //    }
            //    else if (statusCode == 502)
            //    {
            //        msg = "请求错误";
            //    }
            //    else if (statusCode != 200)
            //    {
            //        msg = "未知错误";
            //    }
            //    if (!string.IsNullOrWhiteSpace(msg))
            //    {
            //        await HandleExceptionAsync(context, statusCode, msg);
            //    }
            //}
        }

        private static Task HandleExceptionAsync(HttpContext context, int statusCode, Exception ex)
        {
#if !DEBUG
            var user = "Admin或未知学校";
            var mlUser = JsonHelper.JsonToT<ModelLoginUser>(context.Request.Cookies["UserLogin"]);
            if (mlUser != null)
                user = mlUser.School.SchoolName;
            var stackTrace = ex.StackTrace.Substring(0, ex.StackTrace.IndexOf(" at ", 10));
            var msg = user+ DateTime.Now +"发生错误："+ ex.Message+ stackTrace ;
            LogHelper.Error(msg);
#endif
            //var data = new { code = 0, content = "", errMsg = "发生错误"};
            //var result = JsonHelper.ToJson(data);
            //context.Response.ContentType = "application/json;charset=utf-8";
            var result = "发生错误";
            return context.Response.WriteAsync(result);
        }
    }
}
