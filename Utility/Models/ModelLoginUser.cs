using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utility
{
    public class ModelLoginUser
    {
        /// <summary>
        /// 登录用户类型1:admin,2:school
        /// </summary>
        public byte UserType { get; set; }
        public SYS_Admin Admin { get; set; }
        public SYS_School School{get;set;}
        public short? CityId { get; set; } 
    }
}