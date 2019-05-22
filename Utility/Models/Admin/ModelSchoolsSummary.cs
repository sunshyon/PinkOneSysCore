using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class ModelSchoolsSummary
    {
        public int TotalSchool { get; set; }
        /// <summary>
        /// 累计服务学生数
        /// </summary>
        public long TotalAllStudent { get; set; }
        /// <summary>
        /// 在校学生总数
        /// </summary>
        public long TotalOnSchoolStudent { get; set; }
        public int TotalStaff { get; set; }
        /// <summary>
        /// 累计已发出卡数
        /// </summary>
        public long TotalAllCard { get; set; }
        /// <summary>
        /// 活跃卡数
        /// </summary>
        public long TotalUseCard { get; set; }

    }
}
