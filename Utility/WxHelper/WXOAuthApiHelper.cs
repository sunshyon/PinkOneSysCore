using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    /// <summary>
    /// 微信验证API帮助类
    /// </summary>
    public static class WXOAuthApiHelper
    {
        public static ModelWxSetting mWxSetting = JsonFileProvider.Instance.GetSettings<ModelWxSetting>();

        #region 获取验证地址

        /// <summary>
        /// 获取验证地址
        /// </summary>
        /// <param name="appId">公众号的唯一标识</param>
        /// <param name="redirectUrl">授权后重定向的回调链接地址，请使用urlencode对链接进行处理</param>
        /// <param name="state">重定向后会带上state参数，开发者可以填写a-zA-Z0-9的参数值，最多128字节</param>
        /// <param name="scope">应用授权作用域，snsapi_base （不弹出授权页面，直接跳转，只能获取用户openid），snsapi_userinfo （弹出授权页面，可通过openid拿到昵称、性别、所在地。并且，即使在未关注的情况下，只要用户授权，也能获取其信息）</param>
        /// <param name="responseType">返回类型，请填写code（或保留默认）</param>
        /// <param name="addConnectRedirect">加上后可以解决40029-invalid code的问题（测试中）</param>
        /// <returns></returns>
        public static string GetAuthorizeUrl(string redirectUrl, string state, EnumOAuthScope scope, string responseType = "code", bool addConnectRedirect = true)
        {

            var url =string.Format("https://open.weixin.qq.com/connect/oauth2/authorize?appid={0}&redirect_uri={1}&response_type={2}&scope={3}&state={4}{5}#wechat_redirect",
                                Uri.EscapeDataString(mWxSetting.Base_AppID), Uri.EscapeDataString(redirectUrl), Uri.EscapeDataString(responseType), Uri.EscapeDataString(scope.ToString("g")), Uri.EscapeDataString(state),
                                addConnectRedirect ? "&connect_redirect=1" : "");

            /* 这一步发送之后，客户会得到授权页面，无论同意或拒绝，都会返回redirectUrl页面。
             * 如果用户同意授权，页面将跳转至 redirect_uri/?code=CODE&state=STATE。这里的code用于换取access_token（和通用接口的access_token不通用）
             * 若用户禁止授权，则重定向后不会带上code参数，仅会带上state参数redirect_uri?state=STATE
             */
            return url;
        }

        #endregion

        #region 获取网页验证票据

        /// <summary>
        /// 获取网页验证票据
        /// </summary>
        /// <param name="appId">公众号的唯一标识</param>
        /// <param name="secret">公众号的appsecret</param>
        /// <param name="code">code作为换取access_token的票据，每次用户授权带上的code将不一样，code只能使用一次，5分钟未被使用自动过期。</param>
        /// <param name="grantType">填写为authorization_code（请保持默认参数）</param>
        /// <returns></returns>
        public static ModelWxOAuthToken GetAccessToken(string code, string grantType = "authorization_code")
        {
            String apiUri = string.Format(mWxSetting.Api_GetAuthToken, "&", Uri.EscapeDataString(mWxSetting.Base_AppID), Uri.EscapeDataString(mWxSetting.Base_AppSecret), Uri.EscapeDataString(code), Uri.EscapeDataString(grantType));
            // 获取微信网页认证Token
            return WebApiHelper.GetWxResponse<ModelWxOAuthToken>(mWxSetting.Api_BaseUrl, apiUri);

        }

        #endregion

        #region 获取用户基本信息

        /// <summary>
        /// 获取用户基本信息
        /// </summary>
        /// <param name="accessToken">调用接口凭证</param>
        /// <param name="openId">普通用户的标识，对当前公众号唯一</param>
        /// <param name="lang">返回国家地区语言版本，zh_CN 简体，zh_TW 繁体，en 英语</param>
        /// <returns></returns>
        public static ModelWxUserInfo GetUserInfo(string code, EnumWxLang lang = EnumWxLang.zh_CN)
        {
            // 获取微信网页认证Token
            ModelWxOAuthToken accessToken= GetAccessToken(code);

            String apiUri = string.Format(mWxSetting.Api_GetUserInfo, "&", Uri.EscapeDataString(accessToken.access_token), Uri.EscapeDataString(accessToken.openid), Uri.EscapeDataString(lang.ToString("g")));
            // 获取微信网页认证Token
            return WebApiHelper.GetWxResponse<ModelWxUserInfo>(mWxSetting.Api_BaseUrl, apiUri);

        }

        #endregion

        #region 发送模板消息

        /// <summary>
        /// 发送模板消息
        /// </summary>
        /// <param name="accessToken">调用接口凭证(通用接口accessToken)</param>
        /// <returns></returns>
        public static ModelWmResult SendTmplMessage<T>(string accessTokenFromDb, ModelWxMsg<T> model)
        {
            // API地址
            String apiUri = string.Format(mWxSetting.Api_SendMessage, Uri.EscapeDataString(accessTokenFromDb));
            // 获取微信网页认证Token
            return WebApiHelper.PostWxResponse<ModelWxMsg<T>, ModelWmResult>(mWxSetting.Api_BaseUrl, apiUri, model);
        }

        #endregion


        #region 用户更新微信票据到数据库
        /// <summary>
        /// 获取微信Token
        /// </summary>
        public static ModelWxToken GetWxToken()
        {
            // 设置API路径及参数
            String wxApiUrl = String.Format(mWxSetting.Api_GetToken, "&", mWxSetting.Base_AppID, mWxSetting.Base_AppSecret);
            ModelWxToken token = WebApiHelper.GetWxResponse<ModelWxToken>(mWxSetting.Api_BaseUrl, wxApiUrl);
            return token;
        }
        /// <summary>
        /// 获取微信JsTicket
        /// </summary>
        public static ModelWxJsTicket GetWxJsTicket(ModelWxToken token)
        {
            // 设置API路径及参数
            String wxApiUrl = String.Format(mWxSetting.Api_GetTicket, "&", token.access_token);
            ModelWxJsTicket ticket = WebApiHelper.GetWxResponse<ModelWxJsTicket>(mWxSetting.Api_BaseUrl, wxApiUrl);
            return ticket;
        }
        #endregion
    }
}