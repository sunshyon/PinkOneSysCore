using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataService
{
    public interface ISchoolMngService:IBaseService
    {
        #region 首页用
        string GetUserInfo(int id);
        string GetSchoolData();
        #endregion

        ModelSchoolManage GetSchoolMngModel();
        int AddStaffRole(string roleName, byte roleLevel);
        int DelStaffRole(int id);
        int ModifySchoolInfo(string schoolName, string contact, string address, string newPwd);
        int UploadImgData(string avatarPic);
    }
}
