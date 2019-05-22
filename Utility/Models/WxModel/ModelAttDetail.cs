using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Utility
{
    public class ModelAttDetail
    {
        public long AttId { get; set; }
        public string SchoolName { get; set; }
        public string StuName { get; set; }
        public string ClassName { get; set; }
        public DateTime Time { get; set; }
        public string AttType { get; set; }
        public string AttWay { get; set; }
        public string AttImgPath { get; set; }
    }
}