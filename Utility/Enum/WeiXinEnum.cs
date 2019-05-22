using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Utility
{
    #region 微信请求返回码枚举

    /// <summary>
    /// 微信请求返回
    /// </summary>
    public enum EnumReturnCode
    {
        系统繁忙此时请开发者稍候再试 = -1,
        请求成功 = 0,
        获取access_token时AppSecret错误或者access_token无效 = 40001,
        不合法的凭证类型 = 40002,
        不合法的OpenID = 40003,
        不合法的媒体文件类型 = 40004,
        不合法的文件类型 = 40005,
        不合法的文件大小 = 40006,
        不合法的媒体文件id = 40007,
        不合法的消息类型 = 40008,
        不合法的图片文件大小 = 40009,
        不合法的语音文件大小 = 40010,
        不合法的视频文件大小 = 40011,
        不合法的缩略图文件大小 = 40012,
        不合法的APPID = 40013,
        不合法的access_token = 40014,
        不合法的菜单类型 = 40015,
        不合法的按钮个数1 = 40016,
        不合法的按钮个数2 = 40017,
        不合法的按钮名字长度 = 40018,
        不合法的按钮KEY长度 = 40019,
        不合法的按钮URL长度 = 40020,
        不合法的菜单版本号 = 40021,
        不合法的子菜单级数 = 40022,
        不合法的子菜单按钮个数 = 40023,
        不合法的子菜单按钮类型 = 40024,
        不合法的子菜单按钮名字长度 = 40025,
        不合法的子菜单按钮KEY长度 = 40026,
        不合法的子菜单按钮URL长度 = 40027,
        不合法的自定义菜单使用用户 = 40028,
        不合法的oauth_code = 40029,
        不合法的refresh_token = 40030,
        不合法的openid列表 = 40031,
        不合法的openid列表长度 = 40032,
        不合法的请求字符不能包含uxxxx格式的字符 = 40033,
        不合法的参数 = 40035,
        不合法的请求格式 = 40038,
        不合法的URL长度 = 40039,
        不合法的分组id = 40050,
        分组名字不合法 = 40051,
        缺少access_token参数 = 41001,
        缺少appid参数 = 41002,
        缺少refresh_token参数 = 41003,
        缺少secret参数 = 41004,
        缺少多媒体文件数据 = 41005,
        缺少media_id参数 = 41006,
        缺少子菜单数据 = 41007,
        缺少oauth_code = 41008,
        缺少openid = 41009,
        access_token超时 = 42001,
        refresh_token超时 = 42002,
        oauth_code超时 = 42003,
        需要GET请求 = 43001,
        需要POST请求 = 43002,
        需要HTTPS请求 = 43003,
        需要接收者关注 = 43004,
        需要好友关系 = 43005,
        多媒体文件为空 = 44001,
        POST的数据包为空 = 44002,
        图文消息内容为空 = 44003,
        文本消息内容为空 = 44004,
        多媒体文件大小超过限制 = 45001,
        消息内容超过限制 = 45002,
        标题字段超过限制 = 45003,
        描述字段超过限制 = 45004,
        链接字段超过限制 = 45005,
        图片链接字段超过限制 = 45006,
        语音播放时间超过限制 = 45007,
        图文消息超过限制 = 45008,
        接口调用超过限制 = 45009,
        创建菜单个数超过限制 = 45010,
        回复时间超过限制 = 45015,
        系统分组不允许修改 = 45016,
        分组名字过长 = 45017,
        分组数量超过上限 = 45018,
        不存在媒体数据 = 46001,
        不存在的菜单版本 = 46002,
        不存在的菜单数据 = 46003,
        解析JSON_XML内容错误 = 47001,
        api功能未授权 = 48001,
        用户未授权该api = 50001,
        参数错误invalid_parameter = 61451,
        无效客服账号invalid_kf_account = 61452,
        客服帐号已存在kf_account_exsited = 61453,
        /// <summary>
        /// 客服帐号名长度超过限制(仅允许10个英文字符，不包括@及@后的公众号的微信号)(invalid kf_acount length)
        /// </summary>
        客服帐号名长度超过限制 = 61454,
        /// <summary>
        /// 客服帐号名包含非法字符(仅允许英文+数字)(illegal character in kf_account)
        /// </summary>
        客服帐号名包含非法字符 = 61455,
        /// <summary>
        ///  	客服帐号个数超过限制(10个客服账号)(kf_account count exceeded)
        /// </summary>
        客服帐号个数超过限制 = 61456,
        无效头像文件类型invalid_file_type = 61457,
        系统错误system_error = 61450,
        日期格式错误 = 61500,
        日期范围错误 = 61501,

        //新加入的一些类型，以下文字根据P2P项目格式组织，非官方文字
        发送消息失败_48小时内用户未互动 = 10706,
        发送消息失败_该用户已被加入黑名单_无法向此发送消息 = 62751,
        发送消息失败_对方关闭了接收消息 = 10703,
        对方不是粉丝 = 10700
    }

    #endregion

    #region 微信相关配置节点

    /// <summary>
    /// 微信相关配置节点
    /// </summary>
    public enum EnumWxConfig
    {
        /// <summary>
        /// 微信API服务配置
        /// </summary>
        WxApiSetting,
        /// <summary>
        /// 微信网页AUTH认证配置
        /// </summary>
        WxAuthSetting,
        /// <summary>
        /// 微信票据更新修正时长
        /// </summary>
        WxTokenFixInterval,
        /// <summary>
        /// 微信票据错误时长
        /// </summary>
        WxTokenErrInterval,
        /// <summary>
        /// 微信Cookie周期
        /// </summary>
        WxCookieInterval,
        /// <summary>
        /// 微信票据缓存有效期
        /// </summary>
        WxTokenValidity
    }

    #endregion

    #region 微信API枚举

    /// <summary>
    /// 微信API枚举
    /// </summary>
    public enum EnumWxApi
    {
        /// <summary>
        /// 获取微信票据
        /// </summary>
        GetToken,
        /// <summary>
        /// 获取微信网页认证票据
        /// </summary>
        GetAuthToken,
        /// <summary>
        /// 获取微信用户信息
        /// </summary>
        GetUserInfo,
        /// <summary>
        /// 发送微信信息
        /// </summary>
        SendMessage,
        /// <summary>
        /// 获取客户端脚本票据
        /// </summary>
        GetJsTicket
    }

    #endregion

    #region 应用授权作用域

    /// <summary>
    /// 应用授权作用域
    /// </summary>
    public enum EnumOAuthScope
    {
        /// <summary>
        /// 不弹出授权页面，直接跳转，只能获取用户openid
        /// </summary>
        snsapi_base,
        /// <summary>
        /// 弹出授权页面，可通过openid拿到昵称、性别、所在地。并且，即使在未关注的情况下，只要用户授权，也能获取其信息
        /// </summary>
        snsapi_userinfo
    }

    #endregion

    #region 微信语言

    /// <summary>
    /// 语言
    /// </summary>
    public enum EnumWxLang
    {
        /// <summary>
        /// 中文简体
        /// </summary>
        zh_CN,
        /// <summary>
        /// 中文繁体
        /// </summary>
        zh_TW,
        /// <summary>
        /// 英文
        /// </summary>
        en
    }

    #endregion

    #region 微信状态

    /// <summary>
    /// 微信状态
    /// </summary>
    public enum EnumWxUserStatus
    {
        /// <summary>
        /// 正常用户
        /// </summary>
        正常用户 = 1,
        /// <summary>
        /// 未注册
        /// </summary>
        已注销 = 2,
        /// <summary>
        /// 未缴押金
        /// </summary>
      
    }

    #endregion

    #region 微信支付类型

    /// <summary>
    /// 微信支付类型
    /// </summary>
    public enum EnumPayType
    {
        JSAPI,
        NATIVE,
        APP
    }

    #endregion

    #region 微信签名加密方式

    /// <summary>
    /// 微信签名加密方式
    /// </summary>
    public enum EnumSignType
    {
        MD5
    }

    #endregion

    #region 微信签名加密方式

    /// <summary>
    /// 微信菜单
    /// </summary>
    public enum EnumWxMenu
    {
        /// <summary>
        /// 首页
        /// </summary>
        Home,
        /// <summary>
        /// 地图
        /// </summary>
        Map,
        /// <summary>
        /// 我的
        /// </summary>
        User
    }

    #endregion

    #region 微信Web服务

    /// <summary>
    /// 微信Web服务
    /// </summary>
    public enum EnumWxWebApi
    {
        /// <summary>
        /// 创建服务票据
        /// </summary>
        CreateToken,

    }

    #endregion

    #region 支付模式

    /// <summary>
    /// 支付模式
    /// </summary>
    public enum PayMode
    {
        /// <summary>
        /// 押金
        /// </summary>
        押金 = 1,
        /// <summary>
        /// 充值
        /// </summary>
        充值 = 2,
        /// <summary>
        /// 退款
        /// </summary>
        退款 = 3,
        /// <summary>
        /// 押金
        /// </summary>
        公众号押金 = 4,
        /// <summary>
        /// 充值
        /// </summary>
        公众号充值 = 5,
        /// <summary>
        /// 充值
        /// </summary>
        公众号退款 = 6,
        /// <summary>
        /// 小程序押金
        /// </summary>     
        小程序押金 = 7,
        /// <summary>
        /// 小程序充值
        /// </summary>
        小程序充值 = 8,
        /// <summary>
        /// 小程序退款
        /// </summary>
        小程序退款 = 9,
        /// <summary>
        /// 支付宝转账
        /// </summary>
        支付宝转账 = 10,
        /// <summary>
        /// 手动调账
        /// </summary>
        手动调账 = 11
    }

    #endregion

    #region 支付状态

    /// <summary>
    /// 支付状态
    /// </summary>
    public enum WxPayState
    {
        /// <summary>
        /// 发起付款
        /// </summary>
        发起付款 = 0,
        /// <summary>
        /// 客户端付款成功
        /// </summary>
        客户端付款成功 = 1,
        /// <summary>
        /// 付款成功
        /// </summary>
        付款成功 = 2,
        /// <summary>
        /// 付款失败
        /// </summary>
        付款失败 = 3,
        /// <summary>
        /// 申请退款
        /// </summary>
        申请退款 = 4,
        /// <summary>
        /// 正在退款
        /// </summary>
        正在退款 = 5,
        /// <summary>
        /// 退款成功
        /// </summary>
        退款成功 = 6,
        /// <summary>
        /// 退款失败
        /// </summary>
        退款失败 = 7,
        /// <summary>
        /// 已申请退款
        /// </summary>
        已申请退款 = 8
    }

    #endregion

    #region 微信推送消息类型
    public enum WxMsgType
    {
        发给用户 = 1,
        发给家长 = 2,
        发给职员 = 3,
    }
    #endregion

}