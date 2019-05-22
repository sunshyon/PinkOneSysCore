using System;
using System.Collections.Generic;

namespace Domain
{
    public partial class FK_Stu_Parent
    {
        public long ID { get; set; }
        public long StuId { get; set; }
        public long ParentId { get; set; }
        public string OpenId { get; set; }
        public int SchoolId { get; set; }
        public short? CityId { get; set; }
        public bool Enabled { get; set; }
    }
}
