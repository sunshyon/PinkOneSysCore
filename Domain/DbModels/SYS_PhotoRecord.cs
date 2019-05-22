using System;
using System.Collections.Generic;

namespace Domain
{
    public partial class SYS_PhotoRecord
    {
        public long ID { get; set; }
        public int SchoolId { get; set; }
        public long? MasterId { get; set; }
        public long AlbumId { get; set; }
        public DateTime CreateTime { get; set; }
        public string Describe { get; set; }
        public string ImgUrl { get; set; }
        public string SizeStr { get; set; }
    }
}
