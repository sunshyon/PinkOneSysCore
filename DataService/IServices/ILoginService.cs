using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataService
{
    public interface ILoginService:IBaseService
    {
        ModelLoginUser GetLoginInfo(string name, string pwd);
    }
}
