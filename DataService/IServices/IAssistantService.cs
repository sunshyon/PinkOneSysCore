using System;
using System.Collections.Generic;
using System.Text;
using Utility;

namespace DataService
{
    public interface IAssistantService:IBaseService
    {
        ModelJsonRet GetChatInfo();
    }
}
