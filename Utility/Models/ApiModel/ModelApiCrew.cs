﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class ModelApiStu
    {
        public long StuId { get; set; }
        public string StuName { get; set; }
        public List<string> CardList { get; set; }
    }
    public class ModelApiStaff
    {
        public long StaffId { get; set; }
        public string StaffName { get; set; }
        public List<string> CardList { get; set; }
    }
}
