using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Utility
{
    public class LogHelper
    {
        //在根目录下创建日志目录
        private static string basePath = AppDomain.CurrentDomain.BaseDirectory + "logs";

       /// <summary>
       /// 普通日志
       /// </summary>
        public static void Info(string content,string dicPath="" )
        {
             WriteLog("Info", dicPath, content);
        }

       /// <summary>
       /// 错误日志
       /// </summary>
        public static void Error(string content,string dicPath="" )
        {
             WriteLog("Error", dicPath, content);
        }

        /// <summary>
        /// 实际的写日志操作
        /// type 日志记录类型
        /// </summary>
        protected static void WriteLog(string type, string dicPath, string content)
        {
            var path = basePath;
            if (dicPath.Length > 6)
                path = dicPath;
            if (!Directory.Exists(path))//如果日志目录不存在就创建
            {
                Directory.CreateDirectory(path);
            }

            string time = DateTime.Now.ToString("yyyy/M/d_HH:mm");//获取当前系统时间
            string filename = path + "/" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";//用日期对日志文件命名

            //该方式可以用于频繁写操作，不会出现线程占用报错
            using (FileStream fs = new FileStream(filename, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
            {
                string write_content = time + " " + type + " " + "->" + content+"\r\n";
                var buffer = Encoding.Default.GetBytes(write_content);
                fs.Write(buffer, 0, buffer.Length);
                fs.Flush();
            };
        }


        #region 临时log
        public static void TempLog(string strData)
        {
            string name = DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            string fileDic = AppDomain.CurrentDomain.BaseDirectory + "logs\\Temp\\";
            if (!Directory.Exists(fileDic))
                Directory.CreateDirectory(fileDic);
            string strPath = fileDic + name;
            System.IO.FileStream fs = new System.IO.FileStream(strPath, System.IO.FileMode.Append);
            byte[] data = System.Text.Encoding.Default.GetBytes(strData);
            byte[] line = System.Text.Encoding.ASCII.GetBytes("\r\n");
            fs.Write(data, 0, data.Length);
            fs.Write(line, 0, 2);
            fs.Flush();
            fs.Close();
        }
        #endregion
    }
}
