using System;
using System.Collections.Generic;

namespace Domain
{
    public partial class SYS_Admin
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string OpenId { get; set; }
        public string IdNumber { get; set; }
        public string PersonName { get; set; }
        public string Phone { get; set; }
        public byte? Type { get; set; }
        public string AvatarPic { get; set; }
        public byte? Status { get; set; }
        public DateTime CreateTime { get; set; }
        public string Creater { get; set; }
    }
}
