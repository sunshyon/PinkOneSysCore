using System;
using System.Collections.Generic;

namespace Domain
{
    public partial class SYS_Card
    {
        public long ID { get; set; }
        public string CardNo { get; set; }
        public byte CardType { get; set; }
        public byte Status { get; set; }
        public long CardMasterId { get; set; }
        public int? SchoolId { get; set; }
        public decimal? CardMoney { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Remark { get; set; }
    }
}
