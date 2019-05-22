using System;
using System.Collections.Generic;

namespace Domain
{
    public partial class SYS_Student
    {
        public long ID { get; set; }
        public string StuName { get; set; }
        public string StuNo { get; set; }
        public string IdNumber { get; set; }
        public string Phone { get; set; }
        public DateTime? Birthday { get; set; }
        public byte? Sex { get; set; }
        public string Address { get; set; }
        public byte? Status { get; set; }
        public int SchoolId { get; set; }
        public int? ClassId { get; set; }
        public byte Grade { get; set; }
        public short? CityId { get; set; }
        public string AvatarPic { get; set; }
        public DateTime CreateTime { get; set; }
        public string Creater { get; set; }
        public byte? Type { get; set; }
        public byte? AttStatus { get; set; }
        public string Remark { get; set; }
    }
}
