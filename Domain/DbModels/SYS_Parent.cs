using System;
using System.Collections.Generic;

namespace Domain
{
    public partial class SYS_Parent
    {
        public long ID { get; set; }
        public string OpenId { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public byte? Type { get; set; }
        public string IdNumber { get; set; }
        public string Phone { get; set; }
        public byte RelationType { get; set; }
        public byte? Sex { get; set; }
        public int SchoolId { get; set; }
        public string Address { get; set; }
        public short? CityId { get; set; }
        public byte Status { get; set; }
        public string AvatarPic { get; set; }
        public DateTime? CreatTime { get; set; }
        public string Remark { get; set; }
    }
}
