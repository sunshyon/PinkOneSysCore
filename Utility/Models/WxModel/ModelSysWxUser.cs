using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class ModelSysWxUser
    {
        public SYS_School School { get; set; }
        /// <summary>
        /// 用户类型0：未绑定，1：家长，2：职员，3：家长兼职员
        /// </summary>
        public byte UserType { get; set; }
        public string UserName { get; set; }
        public string AvatarPic { get; set; }
        public string OpenId { get; set; }
        public string Phone { get; set; }
        public string StusJson { get; set; }
    }
}
