using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class ModelSchoolDetail
    {
        public SYS_School School { get; set; }
        public long TotalStudent { get; set; }
        public int TotalStaff { get; set; }
        public int TotalClass { get; set; }
        public long TotalCard { get; set; }

        public Wx_PublicInfo WxPubInfo { get; set; }
    }
}
