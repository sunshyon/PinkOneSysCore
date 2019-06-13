using System;
using System.Collections.Generic;
using System.Text;
using Domain;

namespace Utility
{
     public class ModelStaffDetail
    {
        public SYS_Staff Staff { get; set; }
        public SYS_StaffRole Role { get; set; }
        public List<SYS_Card> Cards { get; set; }
    }
}
