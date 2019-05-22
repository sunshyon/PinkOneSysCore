using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// 微信JS-UI输出信息包
    /// </summary>
    public class ModelWxJsUiPackage
    {
        /// <summary>
        /// 微信AppId
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public string Timestamp { get; set; }
        /// <summary>
        /// 随机码
        /// </summary>
        public string NonceStr { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string Signature { get; set; }
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="appId">APPID</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="nonceStr">随机码</param>
        /// <param name="signature">签名</param>
        public ModelWxJsUiPackage(string appId, string timestamp, string nonceStr, string signature)
        {
            AppId = appId;
            Timestamp = timestamp;
            NonceStr = nonceStr;
            Signature = signature;
        }
    }
}