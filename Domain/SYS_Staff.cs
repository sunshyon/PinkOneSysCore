using System;
using System.Collections.Generic;

namespace Domain
{
    public partial class SYS_Staff
    {
        public int ID { get; set; }
        public string OpenId { get; set; }
        public string WorkNo { get; set; }
        public string StaffName { get; set; }
        public string NickName { get; set; }
        public short? RoleId { get; set; }
        public byte? RoleLevel { get; set; }
        public string Phone { get; set; }
        public string IdNumber { get; set; }
        public int SchoolId { get; set; }
        public string CardNo { get; set; }
        public string AvatarPic { get; set; }
        public byte? Sex { get; set; }
        public short? CityId { get; set; }
        public byte? Status { get; set; }
        public DateTime? CreateTime { get; set; }
        public byte? AttStatus { get; set; }
        public string PinkoneAccount { get; set; }
        public string PinkonePassword { get; set; }
    }
}
