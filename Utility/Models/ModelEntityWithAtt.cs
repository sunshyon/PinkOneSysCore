using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class ModelStudentWithAtt
    {
        public SYS_Student Stu { get; set; }
        public SYS_StudentAttRecord StuAtt{ get; set; }
      
    }
    public class ModelStaffWithAtt
    {
        public SYS_Staff Staff { get; set; }
        public SYS_StaffAttRecord StaffAtt { get; set; }

    }
}
