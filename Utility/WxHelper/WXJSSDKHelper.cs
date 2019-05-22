using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// 微信JSSDK类
    /// </summary>
    public class WXJSSDKHelper
    {
        /// <summary>
        /// 获取随机字符串
        /// </summary>
        /// <returns></returns>
        public static string GetNoncestr()
        {
            var random = new Random();
            return Md5Helper.GetMD5(random.Next(1000).ToString(), "GBK");
        }

        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimestamp()
        {
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// sha1加密
        /// </summary>
        /// <returns></returns>
        private static string CreateSha1(Hashtable parameters)
        {
            var sb = new StringBuilder();
            var akeys = new ArrayList(parameters.Keys);
            akeys.Sort();

            foreach (var k in akeys)
            {
                if (parameters[k] != null)
                {
                    var v = (string)parameters[k];

                    if (sb.Length == 0)
                    {
                        sb.Append(k + "=" + v);
                    }
                    else
                    {
                        sb.Append("&" + k + "=" + v);
                    }
                }
            }
            return SHA1UtilHelper.GetSha1(sb.ToString()).ToLower();
        }

        /// <summary>
        /// 获取JS-SDK权限验证的签名Signature
        /// </summary>
        /// <param name="ticket">客户端票据</param>
        /// <param name="noncestr">随机字串</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="url">当前路径</param>
        /// <returns></returns>
        public static string GetSignature(string ticket, string noncestr, string timestamp, string url)
        {
            var parameters = new Hashtable();
            parameters.Add("jsapi_ticket", ticket);
            parameters.Add("noncestr", noncestr);
            parameters.Add("timestamp", timestamp);
            parameters.Add("url", url);
            return CreateSha1(parameters);
        }

        /// <summary>
        /// 获取给UI使用的JSSDK信息包
        /// </summary>
        /// <param name="appId">应用ID</param>
        /// <param name="appSecret">应用密钥</param>
        /// <param name="ticket">客户端票据</param>
        /// <param name="url">当前路径</param>
        /// <returns></returns>
        public static ModelWxJsUiPackage GetJsSdkUiPackage(string appId, string appSecret, string ticket, string url)
        {
            //获取时间戳
            var timestamp = GetTimestamp();
            //获取随机码
            string nonceStr = GetNoncestr();
            //获取签名
            string signature = GetSignature(ticket, nonceStr, timestamp, url);
            //返回信息包
            return new ModelWxJsUiPackage(appId, timestamp, nonceStr, signature);
        }
    }
}