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
        public static string basePath = AppDomain.CurrentDomain.BaseDirectory + "logs";

        /**
        * 向日志文件写入运行时信息
        * @param className 类名
        * @param content 写入内容
        */
        public static void Info(string content,string dicPath="" )
        {
             WriteLog("Info", dicPath, content);
        }

        /**
        * 向日志文件写入出错信息
        * @param className 类名
        * @param content 写入内容
        */
        public static void Error(string content,string dicPath="" )
        {
             WriteLog("Error", dicPath, content);
        }

        /**
        * 实际的写日志操作
        * @param type 日志记录类型
        * @param className 类名
        * @param content 写入内容
        */
        protected static void WriteLog(string type, string dicPath, string content)
        {
            string path = basePath;
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

            //创建或打开日志文件，向日志文件末尾追加记录
            //StreamWriter mySw = File.AppendText(filename);
            //向日志文件写入内容
            //string write_content = time + " " + type + " " + "->" + content;
            //mySw.WriteLine(write_content);
            //关闭日志文件
            //mySw.Close();
        }
    }
}
