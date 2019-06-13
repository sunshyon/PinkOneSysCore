using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataService
{
    public interface ILocalAppApiService:IBaseService
    {

        bool UploadFile(string name, string path);
        bool DeleteFile(string fullpath);


        ModelJsonRet OAuth(string username, string password, int schoolId);
        ModelJsonRet GetSchoolInfo(string username, string password, int schoolId, string token = "");
        ModelJsonRet UploadAtt(int schoolId, byte personType, long personId, byte attWay, string cardNo, string dateTime, string deviceId, out long attId);


        bool HasRelatedAttRecord(int schoolId, long stuId, string dateTimeStr, out long attId);
        string UpdateAttImg(long id, string fileUrl);

    }
}
