using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Utility;
using Domain;

namespace PinkOneSysCore.Areas.WxRelated.Controllers
{
    public class OAuthController : WxBaseController<IWxService>
    {
        /// <summary>
        /// 陪绮在线自有公众号入口
        /// </summary>
        public ActionResult Index()
        {
            ModelWxSetting mWxSetting = JsonFileProvider.Instance.GetSettings<ModelWxSetting>();

            var wxPubInfo = Service.GetWx_PublicInfo(1);

            // 生成重定向URL
#if DEBUG
            mWxSetting.PubUrl_Host = mWxSetting.PubUrl_Host.Replace("https", "http");
#endif
            String redirectUrl = mWxSetting.PubUrl_Host + mWxSetting.PubUrl_WxHome;
            String authUrl = WXOAuthApiHelper.GetAuthorizeUrl(wxPubInfo.AppId, redirectUrl, "State", EnumOAuthScope.snsapi_userinfo);
            MemoryCacheHelper.SetCache("WxPubInfo", wxPubInfo);
            // 验证跳转

            Response.Redirect(authUrl);
            
            return Content("已完成跳转");
        }

        /// <summary>
        /// 学校公众号入口
        /// </summary>
        /// <param name="sId"></param>
        public ActionResult SchoolPortal(int sId)
        {
            glbSchoolId = sId;

            ModelWxSetting mWxSetting = JsonFileProvider.Instance.GetSettings<ModelWxSetting>();
            var wxPubInfo = Service.GetWx_PublicInfo(2,sId);
            // 生成重定向URL
            String redirectUrl = mWxSetting.PubUrl_Host + mWxSetting.PubUrl_WxHome;
            String authUrl = WXOAuthApiHelper.GetAuthorizeUrl(wxPubInfo.AppId,redirectUrl, "State", EnumOAuthScope.snsapi_userinfo);
            MemoryCacheHelper.SetCache("WxPubInfo", wxPubInfo);
            // 验证跳转
            Response.Redirect(authUrl);

            return Content("已完成跳转");
        }
    }
}