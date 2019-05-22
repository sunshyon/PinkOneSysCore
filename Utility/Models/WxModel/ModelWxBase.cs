using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// 微信基类
    /// </summary>
    public class ModelWxBase
    {
        /// <summary>
        /// 错误码
        /// </summary>
        public int errcode { get; set; } = 0;
        /// <summary>
        /// 错误信息
        /// </summary>
        public string errmsg { get; set; } = string.Empty;
    }
}