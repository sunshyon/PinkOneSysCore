using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class ModelStuDetail
    {
        public SYS_Student Stu { get; set; }
        public SYS_Class Class { get; set; }
        public List<SYS_Parent> Parents { get; set; }
        public List<SYS_Card> Cards { get; set; }
    }
}
