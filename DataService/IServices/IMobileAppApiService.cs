using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace DataService
{
    public interface IMobileAppApiService
    {
        ModelJsonRet MobileAppLogin(byte type, string username, string password);
        ModelJsonRet UploadAttFromApp(int schoolId, long stuId, byte attType, byte attWay, byte isSendMsg);
    }
}
