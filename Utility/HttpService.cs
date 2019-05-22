using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Utility
{
    public class HttpService
    {
        private static int timeout = 10 * 1000*5;
        public static string PostUrl(string url, string postJson)
        {
            string res = "";
            try
            {
                HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
                req.Method = "POST";
                req.Timeout = timeout;//设置请求超时时间，单位为毫秒 
                req.ContentType = "application/json";
                //Dictionary<string, string> dic = JsonHelper.JsonToT<Dictionary<string, string>>(postJson);
                //req.Headers.Add("user", dic["user"]);
                //req.Headers.Add("pwd", dic["pwd"]);

                byte[] data = Encoding.UTF8.GetBytes(postJson);
                req.ContentLength = data.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(data, 0, data.Length);
                    reqStream.Close();
                }
                HttpWebResponse resp = (HttpWebResponse)req.GetResponse();
                Stream stream = resp.GetResponseStream();
                //获取响应内容 
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                { res = reader.ReadToEnd(); }
                resp.Close();
                req.Abort();
            }
            catch(Exception ex)
            {
                var str = "PostUrl：" + DateTime.Now + "，url:" + url+ "，错误" + ex.Message;
                LogHelper.Error(str);
            }
            return res;
        }
        public static string UpLoadFileWithParam(string url, string postJson, string filePath)
        {
            var res = "";
            try
            {
                // 边界符  
                var boundary = "---------------" + DateTime.Now.Ticks.ToString("x");
                var beginBoundary = Encoding.ASCII.GetBytes("--" + boundary + "\r\n");
                var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

                // 最后的结束符  
                var endBoundary = Encoding.ASCII.GetBytes("--" + boundary + "--\r\n");

                // 文件参数头  
                const string filePartHeader =
                    "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\n" +
                     "Content-Type: application/octet-stream\r\n\r\n";
                var fileHeader = string.Format(filePartHeader, "file", "test");
                var fileHeaderBytes = Encoding.UTF8.GetBytes(fileHeader);

                // 开始拼数据  
                var memStream = new MemoryStream();
                memStream.Write(beginBoundary, 0, beginBoundary.Length);

                // 文件数据  
                memStream.Write(fileHeaderBytes, 0, fileHeaderBytes.Length);
                var buffer = new byte[1024];
                int bytesRead; // =0  
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    memStream.Write(buffer, 0, bytesRead);
                }
                fileStream.Close();

                // Key-Value数据  
                var stringKeyHeader = "\r\n--" + boundary +
                                       "\r\nContent-Disposition: form-data; name=\"{0}\"" +
                                       "\r\n\r\n{1}\r\n";

                //写入参数
                Dictionary<string, string> stringDict = new Dictionary<string, string>();
                stringDict = JsonHelper.JsonToT<Dictionary<string, string>>(postJson);
                stringDict.Add("paramEnd", "paramEnd");

                foreach (byte[] formitembytes in from string key in stringDict.Keys
                                                 select string.Format(stringKeyHeader, key, stringDict[key])
                                                     into formitem
                                                 select Encoding.UTF8.GetBytes(formitem))
                {
                    memStream.Write(formitembytes, 0, formitembytes.Length);
                }

                // 写入最后的结束边界符  
                memStream.Write(endBoundary, 0, endBoundary.Length);

                //倒腾到tempBuffer  
                memStream.Position = 0;
                var tempBuffer = new byte[memStream.Length];
                memStream.Read(tempBuffer, 0, tempBuffer.Length);
                memStream.Close();

                var str = Encoding.UTF8.GetString(tempBuffer);

                // 创建webRequest并设置属性  
                var webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.Method = "POST";
                webRequest.Timeout = timeout*10;
                webRequest.ContentType = "multipart/form-data; boundary=" + boundary;
                webRequest.ContentLength = tempBuffer.Length;

                //写入requestStream
                var requestStream = webRequest.GetRequestStream();
                requestStream.Write(tempBuffer, 0, tempBuffer.Length);
                requestStream.Close();

                //接收返回
                var httpWebResponse = (HttpWebResponse)webRequest.GetResponse();
                using (var httpStreamReader = new StreamReader(httpWebResponse.GetResponseStream(), Encoding.GetEncoding("utf-8")))
                {
                    res = httpStreamReader.ReadToEnd();
                }

                httpWebResponse.Close();
                webRequest.Abort();
            }
            catch (Exception ex)
            {
                var str = "UpLoadFileWithParam：" + DateTime.Now + "，url:" + url + "，错误" + ex.Message;
                LogHelper.Error(str);
            }
            return res;
        }

    }
}
