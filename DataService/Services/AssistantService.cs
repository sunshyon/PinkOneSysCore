using Domain;
using System;
using System.Collections.Generic;
using System.Text;
using Utility;
using System.Linq;

namespace DataService
{
    public class AssistantService:BaseService, IAssistantService
    {
        #region 聊天相关
        public List<SYS_Staff> GetHasAcutStaffs()
        {
            return UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID && x.PinkoneAccount.Length > 6 && x.Status == (byte)StaffStatus.在职).Result;
        }
        public ModelJsonRet GetChatInfo()
        {
            var school = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID == mlUser.School.ID).Result.FirstOrDefault();
            var hasAcutStaffs = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.SchoolId == mlUser.School.ID && x.PinkoneAccount.Length > 6&&x.Status==(byte)StaffStatus.在职).Result;
            var sb = new StringBuilder();
            //sb.Append("<div class='media media-single'><img class='avatar avatar-xl' src='" + school.AvatarPic + "'>");
            //sb.Append("<div class='media-body'><h5>" + school.SchoolName + "</h5><small class='user-state' id='schoolState" + school.ID + "'>离线</small></div></div>");
            foreach(var s in hasAcutStaffs)
            {
                var avatar = s.AvatarPic;
                if (s.AvatarPic == null || s.AvatarPic.Length < 6)
                {
                    avatar = "/Images/unknown_user_avatar.jpg";
                }
                sb.Append("<div class='media media-single'><img class='avatar avatar-xl' src='" + avatar + "'>");
                sb.Append("<div class='media-body'><h5>" + s.StaffName + "</h5><small class='user-state' id='staffState" + s.ID + "'>离线</small></div></div>");
            }
            var userSelf = new
            {
                schoolId = school.ID,
                userName = school.SchoolName,
                staffId = 0,
                avatar = school.AvatarPic
            };
            if (mlUser.Staff != null)
            {
                userSelf = new
                {
                    schoolId = school.ID,
                    userName = mlUser.Staff.StaffName,
                    staffId = mlUser.Staff.ID,
                    avatar = mlUser.Staff.AvatarPic
                };
            }
            var josn = new
            {
                userSelf,
                userListHtml = sb.ToString()
            };
            mjRet.code = 1;
            mjRet.content = josn;
            return mjRet;
        }
        #endregion
    }
}
