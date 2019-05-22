using System;
using System.Collections.Generic;

namespace Domain
{
    public partial class SYS_School
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string SchoolName { get; set; }
        public byte Status { get; set; }
        public byte? Type { get; set; }
        public string Address { get; set; }
        public string ContactInfo { get; set; }
        public string AvatarPic { get; set; }
        public short? CityId { get; set; }
        public string AssociatedStaffOpenId1 { get; set; }
        public string AssociatedStaffOpenId2 { get; set; }
        public string AssociatedStaffOpenId3 { get; set; }
        public string AssociatedStaffOpenId4 { get; set; }
        public string AssociatedStaffOpenId5 { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Token { get; set; }
    }
}
