using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataService
{
    public interface IWxService:IBaseService
    {
        Wx_PublicInfo GetWx_PublicInfo(byte type, int schoolId = 0);
        ModelSysWxUser GetSysWxUserModelTest();
        void UpdateWxUserInfo(ModelWxUserInfo wxUserInfo);
        ModelSysWxUser GetSysWxUserModel(int sId, string openId);
        string GetStudentByName(string stuName, string schoolName);
        int BindAndRegisterParent(string stusJson, string parentJson, ModelWxUserInfo wxUser);


        string GetStaffByInfo(string staffInfo, string schoolName);
        int BindOrRegisterStaff(int staffId, string staffJson, ModelWxUserInfo wxUser);
        ModelAttDetail GetStuAttDatail(long attId);
        string GetStuAttImgPath(long attId);


        string GetWxBindedJson(string openId);
        int DelWxBinded(byte type, long id);
        string DoWxBind(string name, string cardNo, ModelWxUserInfo wxUser);
    }
}
