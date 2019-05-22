using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posinda.Model.Wx
{
    /// <summary>
    /// 微信支付基础信息存储类
    /// </summary>
    public class ModelWxPayInfo
    {
        /// <summary>
        /// 第三方用户唯一凭证appid
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 第三方用户唯一凭证密钥，即appsecret
        /// </summary>
        public string AppSecret { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        public string MchId { get; set; }
        /// <summary>
        /// 商户支付密钥Key。登录微信商户后台，进入栏目【账户设置】【密码安全】【API 安全】【API 密钥】
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// 支付完成后的回调处理页面
        /// </summary>
        public string NotifyUrl { get; set; }
    }
}