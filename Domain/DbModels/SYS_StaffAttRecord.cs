using System;
using System.Collections.Generic;

namespace Domain
{
    public partial class SYS_StaffAttRecord
    {
        public long ID { get; set; }
        public int SchoolId { get; set; }
        public int MasterId { get; set; }
        public byte AttType { get; set; }
        public DateTime AttTime { get; set; }
        public string AttTimeStr { get; set; }
        public byte? AttWay { get; set; }
        public double? Temperature { get; set; }
        public string Remark { get; set; }
    }
}
