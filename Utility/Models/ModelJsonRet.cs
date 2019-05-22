using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class ModelJsonRet
    {
        /// <summary>
        /// 结果码->0:失败，1:成功
        /// </summary>
        public byte code { get; set; }
        /// <summary>
        /// 错误提示
        /// </summary>
        public string errMsg { get; set; }
        /// <summary>
        /// 成功内容
        /// </summary>
        public object content { get; set; }
    }
}
