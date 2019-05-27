using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataService
{
    public interface ICrewService:IBaseService
    {
        string GetStuInfo(string nqy, int cqy);
        string AddOrModifyStu( byte type, string entity);
        string GetStuById(int id);
        int DelStuById(long id);

        ModelStuDetail GetStuDetailModel(long stuId);
        string ImportStus(int classId, string fileName);



        string GetStaffInfo(string query);
        string AddOrModifyStaff( byte type, string entity);
        string GetStaffById(int id);
        int DelStaffById(long id);

        ModelJsonRet AddStaffPinkoneAccount(int staffId, string account, string password);
        ModelStaffDetail GetStaffDetailModel(int staffId);
        string ImportStaffs(string fileName);



        string GetClassInfo();
        int AddOrModifyClass( byte type, string entity);
        string GetClassById(int id);
        int DelClassById(long id);

       

    }
}
