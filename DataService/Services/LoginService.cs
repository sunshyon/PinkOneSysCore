﻿using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataService
{
    public class LoginService:BaseService, ILoginService
    {
        /// <summary>
        /// accountType->1：学校账号，2：老师账号
        /// </summary>
        public ModelLoginUser GetUserLoginInfo(byte accountType, string name,string pwd)
        {
            var mlu=new ModelLoginUser();
            if (accountType == 1)
            {
                var school= UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.Username.Equals(name) && x.Password.Equals(pwd)).Result.FirstOrDefault();
                if (school != null)
                {
                    school.AvatarPic = null;
                    mlu.UserType = 1;
                    mlu.School = school;
                }
            }
            else
            {
                var staff = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.PinkoneAccount.Equals(name) && x.PinkonePassword == pwd&&x.Status==(byte)StaffStatus.在职).Result.FirstOrDefault();
                if (null != staff)
                {
                    var school = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID==staff.SchoolId).Result.FirstOrDefault();
                    if (null != school)
                    {
                        mlu.Staff = staff;
                        school.AvatarPic = null;
                        mlu.School = school;
                        mlu.UserType = 2;
                        mlu.CityId = school.CityId;
                    }
                }
            }
            mlUser = mlu;
            return mlUser;
        }

        public SYS_Admin GetAdminLoginInfo(string name, string pwd)
        {
            return UnitOfWork.Repository<SYS_Admin>().GetEntitiesAsync(x => x.Username == name && x.Password == pwd && x.Status == 1).Result.FirstOrDefault();
        }
    }
}
