using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// 配置帮助类
    /// </summary>
    public static class ConfigHelper
    {
        #region 变量
        private static IConfiguration _configuration;
        #endregion

        #region 构造函数
        static ConfigHelper()
        {
            //在当前目录或者根目录中寻找appsettings.json文件
            var fileName = "appsettings.json";

            var directory = AppContext.BaseDirectory;
            directory = directory.Replace("\\", "/");

            var filePath = $"{directory}/{fileName}";
            if (!File.Exists(filePath))
            {
                var length = directory.IndexOf("/bin");
                filePath = $"{directory.Substring(0, length)}/{fileName}";
            }

            var builder = new ConfigurationBuilder()
                .AddJsonFile(filePath, false, true);

            _configuration = builder.Build();
        }
        #endregion

        #region 方法
        /// <summary>
        /// 获取配置信息数据
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public static string AppSettings(string key)
        {
            return _configuration.GetSection("AppSettings:"+key).Value;
        }

       
        #endregion
    }
}