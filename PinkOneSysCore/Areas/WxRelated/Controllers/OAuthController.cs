using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace PinkOneSysCore.Areas.WxRelated.Controllers
{
    public class OAuthController : WxBaseController<IWxService>
    {
        public enum OauthType
        {
            主页 = 1,
            个人中心 = 2,
            校园卡关联 = 3
        }
        /// <summary>
        /// 陪绮在线自有公众号入口
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult Index(byte type)
        {
            ModelWxSetting mWxSetting = JsonFileProvider.Instance.GetSettings<ModelWxSetting>();

            // 生成重定向URL
            String redirectUrl = mWxSetting.PubUrl_Host + mWxSetting.PubUrl_WxHome;
            if(type==(byte)OauthType.个人中心)
                redirectUrl=mWxSetting.PubUrl_Host+ mWxSetting.PubUrl_Center;
            else if (type == (byte)OauthType.校园卡关联)
                redirectUrl = mWxSetting.PubUrl_Host + mWxSetting.PubUrl_WxBind;
            else if (type == (byte)OauthType.主页)
                redirectUrl = mWxSetting.PubUrl_Host + mWxSetting.PubUrl_WxHome;

            String authUrl = WXOAuthApiHelper.GetAuthorizeUrl(redirectUrl, "State", EnumOAuthScope.snsapi_userinfo);

            // 验证跳转
            Response.Redirect(authUrl);
            
            return Content("提前完成跳转");
        }

        /// <summary>
        /// 学校公众号入口
        /// </summary>
        /// <param name="sId"></param>
        /// <returns></returns>
        public ActionResult SchoolPortal(int sId)
        {
            glbSchoolId = sId;

            ModelWxSetting mWxSetting = JsonFileProvider.Instance.GetSettings<ModelWxSetting>();

            // 生成重定向URL
            String redirectUrl = mWxSetting.PubUrl_Host + mWxSetting.PubUrl_WxHome;
           
            redirectUrl = mWxSetting.PubUrl_Host + mWxSetting.PubUrl_WxHome;

            String authUrl = WXOAuthApiHelper.GetAuthorizeUrl(redirectUrl, "State", EnumOAuthScope.snsapi_userinfo);

            // 验证跳转
            Response.Redirect(authUrl);

            return Content("提前完成跳转");
        }
    }
}