using Domain;
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
        public ModelLoginUser GetLoginInfo(string name,string pwd)
        {
            var mlu=new ModelLoginUser();
            SYS_Admin admin = UnitOfWork.Repository<SYS_Admin>().GetEntitiesAsync(x => x.Password.Equals(pwd) && x.Username==name).Result.FirstOrDefault();
            if (null == admin)
            {
                var t = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.Username.Equals(name) && x.Password.Equals(pwd));
                SYS_School school = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.Username.Equals(name) && x.Password.Equals(pwd)).Result.FirstOrDefault();
                if (null != school)
                {
                    school.AvatarPic = null;
                    mlu.School = school;
                    mlu.UserType = 2;
                    mlu.CityId = school.CityId;
                }
            }
            else
            {
                mlu.Admin = admin;
                mlu.UserType = 1;
            }
            mlUser = mlu;
            //CookieSessionHelper.AddCookie(ComConst.Cookie_LoginUser, JsonHelper.ToJson(mlUser));
            return mlUser;
        }

    }
}
