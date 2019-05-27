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
        /// 登录用户类型1:school,2:staff
        /// </summary>
        public byte UserType { get; set; }
        public SYS_School School{get;set;}
        public SYS_Staff Staff { get; set; }
        public short? CityId { get; set; } 
    }
}