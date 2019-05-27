using System;
using System.Collections.Generic;

namespace Domain
{
    public partial class SYS_Class
    {
        public int ID { get; set; }
        public string ClassNo { get; set; }
        public string ClassName { get; set; }
        public byte Grade { get; set; }
        public int SchoolId { get; set; }
        public int? ClassTeacherId1 { get; set; }
        public int? ClassTeacherId2 { get; set; }
        public int? ClassTeacherId3 { get; set; }
        public int? ClassTeacherId4 { get; set; }
        public byte Status { get; set; }
    }
}
