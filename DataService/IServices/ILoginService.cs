using Domain;
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
        ModelLoginUser GetUserLoginInfo(byte accountType, string name, string pwd);
        SYS_Admin GetAdminLoginInfo(string name, string pwd);
    }
}
