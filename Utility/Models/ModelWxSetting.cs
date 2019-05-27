using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class ModelWxSetting
    {
        /// <summary>
        /// 微信接口
        /// </summary>
        public string Api_BaseUrl { get; set; }
        public string Api_GetToken { get; set; }
        public string Api_GetAuthToken { get; set; }
        public string Api_GetUserInfo { get; set; }
        public string Api_SendMessage { get; set; }
        public string Api_GetTicket { get; set; }

        /// <summary>
        /// 公众号主机域名
        /// </summary>
        public string PubUrl_Host { get; set; }
        public string PubUrl_WxHome { get; set; }
        public string PubUrl_Center { get; set; }
        public string PubUrl_WxBind { get; set; }
        public string PubUrl_AttDetail { get; set; }
    }
    
}
