using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataService
{
    public interface IAdminService:IBaseService
    {
        ModelSchoolsSummary GetSchoolsSummerModel();

        string GetSchoolsInfo(string query);
        string AddOrModifySchool(byte type, string entity);
        string GetSchoolById(int id);
        int DelSchoolById(long id);
        ModelJsonRet BindWxPub(byte type, int schoolId, string json);

        string GetAdminSettingInfo(byte type);
        int AddStaffBaseRole(string roleName, byte roleLevel);
        int DelStaffBaseRole(int id);
        ModelSchoolDetail GetSchoolDetailModel(int id);

        string AddNewAdmin(string username, string password, string personName, string phone);
        int DelAdmin(int id);
    }
}
