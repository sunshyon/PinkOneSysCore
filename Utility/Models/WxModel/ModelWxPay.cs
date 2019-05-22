using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posinda.Model.Wx
{
    /// <summary>
    /// 微信支付
    /// </summary>
    public class ModelWxPay
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        public string appId { get; set; }
        /// <summary>
        /// 支付时间戳
        /// </summary>
        public string timeStamp { get; set; }
        /// <summary>
        /// 随机串
        /// </summary>
        public string nonceStr { get; set; }
        /// <summary>
        /// 扩展包
        /// </summary>
        public string package { get; set; }
        /// <summary>
        /// 微信签名方式
        /// </summary>
        public string signType { get; set; }
        /// <summary>
        /// 微信签名
        /// </summary>
        public string paySign { get; set; }
    }
}