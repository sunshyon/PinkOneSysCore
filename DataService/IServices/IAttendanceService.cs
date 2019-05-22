using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataService
{
    public interface IAttendanceService: IBaseService
    {
        string GetClassesAttNow();
        string GetStuAttNow(string nqy, int cqy);
        int AddStuAttManually(long stuId, byte attType, string attTime,string attTemp, string attRemark, byte isSendWxMsg = 1);
        string GetStuAttDetails(string nqy, int cqy, string sTime, string eTime);
        string GetStuAttMouth(int cqy, string mouth);


        string GetStaffAttNow(string nqy);
        int AddStaffAttManually(int staffId, byte attType, string attTime, string attRemark);
        string GetStaffAttDetails(string nqy, string sTime, string eTime);
        string GetStaffAttMouth(string mouth);
    }
}
