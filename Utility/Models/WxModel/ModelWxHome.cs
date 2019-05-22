using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// 微信用户首页
    /// </summary>
    public class ModelWxHome
    {
        /// <summary>
        /// 跳转URL
        /// </summary>
        public String RedirectUri { get; set; }
        /// <summary>
        /// JS信息输出包
        /// </summary>
        public ModelWxJsUiPackage JsUiPackge { get; set; }
    }
}