using System;
using System.Collections.Generic;

namespace Domain
{
    public partial class SYS_Notice
    {
        public int ID { get; set; }
        public int SchoolId { get; set; }
        public byte Type { get; set; }
        public byte? Status { get; set; }
        public byte? NoticeLevel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Message { get; set; }
    }
}
