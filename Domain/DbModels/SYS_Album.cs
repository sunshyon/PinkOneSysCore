using System;
using System.Collections.Generic;

namespace Domain
{
    public partial class SYS_Album
    {
        public long ID { get; set; }
        public int SchoolId { get; set; }
        public long? MasterId { get; set; }
        public int? ClassId { get; set; }
        public byte Type { get; set; }
        public string AlbumName { get; set; }
        public DateTime? CreateTime { get; set; }
        public string Preview { get; set; }
        public int PhotoCount { get; set; }
        public DateTime? UpdateTime { get; set; }
    }
}
