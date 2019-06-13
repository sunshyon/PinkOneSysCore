using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace PinkOneSysCore.Areas.WxRelated.Controllers
{
    public class WxHomeController : WxBaseController<IWxService>
    {
        [OAuthFilter]
        public ActionResult Index()
        {
            ViewBag.SchoolName = "陪绮在线";
            var mWxUserInfo = JsonHelper.JsonToT<ModelWxUserInfo>(GetSession(ComConst.Wx_ModelWxUserInfo));
           
            var mSysWxUser = new ModelSysWxUser()
            {
                UserType = 0,
            };

            if (mWxUserInfo != null)
            {
                Service.UpdateWxUserInfo(mWxUserInfo);//更新微信用户信息
                mSysWxUser = Service.GetSysWxUserModel(glbSchoolId, mWxUserInfo.openid);
                SetCookie(ComConst.Wx_ModelSysWxUser, JsonHelper.ToJson(mSysWxUser));
                HttpContextCore.SetSession(ComConst.Wx_ModelSysWxUser, JsonHelper.ToJson(mSysWxUser));

                if (mSysWxUser.School != null)
                    ViewBag.SchoolName = mSysWxUser.School.SchoolName;
            }
            return View("Index_WxHome", mSysWxUser);
        }
    }
}