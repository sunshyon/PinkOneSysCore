using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// Web服务调用帮助类
    /// </summary>
    public class WebApiHelper
    {
        #region 微信服务调用
        /// <summary>
        /// GET模式获取微信API返回
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="url">Web服务地址</param>
        /// <param name="apiUrl">API服务对象地址（含参数）</param>
        /// <returns></returns>
        public static T GetWxResponse<T>(string url, string apiUrl)
            where T : class, new()
        {
            // 如果是HTTPS请求，需要设置安全级别
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            using (HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip })
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage response = client.GetAsync(apiUrl).Result;
                    T result = default(T);
                    if (response.IsSuccessStatusCode)
                    {
                        string resultString = response.Content.ReadAsStringAsync().Result;
                        result = JsonHelper.JsonToT<T>(resultString);
                    }
                    response.Dispose();
                    return result;
                }
            }
        }

        /// <summary>
        /// Post模式提交微信API
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="url">Web服务地址</param>
        /// <param name="apiUrl">API服务对象地址（含参数）</param>
        /// <returns></returns>
        public static TResult PostWxResponse<T, TResult>(string url, string apiUrl, T model)
            where T : class, new()
            where TResult : class, new()
        {
            // 如果是HTTPS请求，需要设置安全级别
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;
            using (HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip })
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var json = JsonHelper.ToJson(model);
                    HttpContent httpContent = new StringContent(json);
                    HttpResponseMessage response
                        = client.PostAsync(apiUrl, httpContent).Result;
                    TResult result = default(TResult);
                    if (response.IsSuccessStatusCode)
                    {
                        string resultString = response.Content.ReadAsStringAsync().Result;
                        result = JsonHelper.JsonToT<TResult>(resultString);
                    }
                    response.Dispose();
                    return result;
                }
            }
        }

        /// <summary>
        /// V3接口全部为Xml形式，故有此方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="xmlString"></param>
        /// <returns></returns>
        public static String PostXmlResponse(string url, string apiUrl, string xmlString)
        {
            // 如果是HTTPS请求，需要设置安全级别
            if (url.StartsWith("https"))
                System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls;

            using (HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip })
            {
                using (HttpClient client = new HttpClient(handler))
                {
                    client.BaseAddress = new Uri(url);
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpContent httpContent = new StringContent(xmlString);
                    HttpResponseMessage response
                        = client.PostAsync(apiUrl, httpContent).Result;
                    string resultString = string.Empty;
                    if (response.IsSuccessStatusCode)
                    {
                        resultString = response.Content.ReadAsStringAsync().Result;
                        //result = JsonHelper.FromJson<TResult>(resultString);
                    }
                    response.Dispose();
                    return resultString;
                }
            }
        }

        #endregion
    }
}