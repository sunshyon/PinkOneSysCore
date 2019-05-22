using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Posinda.Model.Wx
{
    public class ModelWxPublicMenuItem
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string type { get; set; }
        /// <summary>
        /// 链接地址
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 子菜单
        /// </summary>
        public List<ModelWxPublicMenuItem> sub_button { get; set; }
    }
}
