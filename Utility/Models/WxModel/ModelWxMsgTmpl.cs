using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 微信消息及模板
/// 全部微信消息模板对应类，在这里定义，
/// 用#region..............#endregion划分
/// </summary>
namespace Utility
{
    #region 消息实体

    /// <summary>
    /// 消息实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ModelWxMsg<T>
    {
        /// <summary>
        /// 消息接收人OpenID
        /// </summary>
        public string touser { get; set; }
        /// <summary>
        /// 消息模板ID
        /// </summary>
        public string template_id { get; set; }
        /// <summary>
        /// 消息模板跳转地址
        /// </summary>
        public string url { get; set; }
        /// <summary>
        /// 消息模板标题颜色
        /// </summary>
        public string topcolor { get; set; }
        /// <summary>
        /// 消息模板类
        /// </summary>
        public T data { get; set; }
    }

    #endregion

    #region 发送消息返回实体

    /// <summary>
    /// 消息实体
    /// </summary>
    public class ModelWmResult : ModelWxBase
    {
        /// <summary>
        /// 消息ID
        /// </summary>
        public Int64 msgid { get; set; }
    }

    #endregion

    #region 模板 - 元素

    /// <summary>
    /// 微信消息模板元素
    /// </summary>
    public class ModelWmElement
    {
        /// <summary>
        /// 值
        /// </summary>
        public string value { get; set; }
        /// <summary>
        /// 颜色（例：#173177）
        /// </summary>
        public string color { get; set; }
    }

    #endregion

    #region 模板 - 基类

    /// <summary>
    /// 微信消息模板元素
    /// </summary>
    public class ModelWmTmpl
    {
        /// <summary>
        /// 标题
        /// </summary>
        public ModelWmElement first { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public ModelWmElement remark { get; set; }
    }

    #endregion

    #region 模板 - 微信推送考勤信息给家长
    public class ModelWmAttendance: ModelWmTmpl
    {
        /// <summary>
        /// 学生名
        /// </summary>
        public ModelWmElement keyword1 { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        public ModelWmElement keyword2 { get; set; }
        /// <summary>
        /// 学校
        /// </summary>
        public ModelWmElement keyword3 { get; set; }

    }
    #endregion

    #region 模板 - 微信扫码租车开通成功通知

    /// <summary>
    /// 微信扫码租车开通成功通知
    /// </summary>
    public class ModelWmOpened : ModelWmTmpl
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public ModelWmElement keyword1 { get; set; }
        /// <summary>
        /// 租车押金
        /// </summary>
        public ModelWmElement keyword2 { get; set; }
        /// <summary>
        /// 开通时间（例：2016/5/5 15:21:01）
        /// </summary>
        public ModelWmElement keyword3 { get; set; }
    }

    #endregion

    #region 模板 - 微信扫码租车充值成功通知

    /// <summary>
    ///  微信扫码租车充值成功通知
    /// </summary>
    public class ModelPaySuccess : ModelWmTmpl
    {
        /// <summary>
        /// 充值金额
        /// </summary>
        public ModelWmElement keyword1 { get; set; }
        /// <summary>
        /// 充值时间
        /// </summary>
        public ModelWmElement keyword2 { get; set; }

    }

    #endregion

}