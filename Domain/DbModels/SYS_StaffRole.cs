using System;
using System.Collections.Generic;

namespace Domain
{
    public partial class SYS_StaffRole
    {
        public short ID { get; set; }
        public int SchoolId { get; set; }
        public string RoleName { get; set; }
        public byte RoleLevel { get; set; }
        public string Remark { get; set; }
    }
}
