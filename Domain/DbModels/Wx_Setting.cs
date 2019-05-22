using System;
using System.Collections.Generic;

namespace Domain
{
    public partial class Wx_Setting
    {
        public string ID { get; set; }
        public string AppId { get; set; }
        public string AppName { get; set; }
        public string AppSecret { get; set; }
        public DateTime? UpdateTime { get; set; }
        public string AccessToken { get; set; }
        public string JsApiTicket { get; set; }
    }
}
