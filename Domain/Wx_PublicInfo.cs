using System;
using System.Collections.Generic;

namespace Domain
{
    public partial class Wx_PublicInfo
    {
        public int ID { get; set; }
        public byte Type { get; set; }
        public int? SchoolId { get; set; }
        public string AppId { get; set; }
        public string AppName { get; set; }
        public string AppSecret { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string AccessToken { get; set; }
        public string JsApiTicket { get; set; }
        public string AttRetTempId { get; set; }
    }
}
