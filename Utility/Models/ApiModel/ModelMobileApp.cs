using System;
using System.Collections.Generic;
using System.Text;

namespace Utility
{
    public class ModelMobileAppSchool
    {
        public int ID { get; set; }
        public string SchoolName { get; set; }
        public string ContactInfo { get; set; }
        public string Token { get; set; }
    }
    public class ModelMobileAppStu
    {
        public long ID { get; set; }
        public string StuName { get; set; }
        public string StuNo { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public byte? Sex { get; set; }
        public string Address { get; set; }
        public int SchoolId { get; set; }
        public int? ClassId { get; set; }
        public byte Grade { get; set; }
        public string AvatarPic { get; set; }
        //public byte? AttStatus { get; set; }
    }
    public class ModelMobileAppStaff
    {
        public int ID { get; set; }
        public string WorkNo { get; set; }
        public string StaffName { get; set; }
        public byte? RoleLevel { get; set; }
        public string Phone { get; set; }
        public int SchoolId { get; set; }
        public string AvatarPic { get; set; }
        //public byte? Sex { get; set; }
        //public byte? AttStatus { get; set; }
        public string PinkoneAccount { get; set; }
        public string PinkonePassword { get; set; }
    }
}
