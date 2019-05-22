using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace PinkOneSysCore.Areas.WxRelated.Controllers
{
    public class CenterController : WxBaseController<IWxService>
    {
        /// <summary>
        /// 我的主页
        /// </summary>
        [WxSysUserFilter]
        public ActionResult Index()
        {
            //测试用
            mSysWxUser = Service.GetSysWxUserModelTest();
            return View("Index_Center", mSysWxUser);
        }
        /// <summary>
        /// 关联绑定页面
        /// </summary>
        public ActionResult WxBind()
        {
            //测试用
            mSysWxUser = Service.GetSysWxUserModelTest();
            if (mSysWxUser != null && mSysWxUser.OpenId.Length > 6)
                ViewBag.WxBindedJson = Service.GetWxBindedJson(mSysWxUser.OpenId);
            return View("Index_WxBind");
        }
        /// <summary>
        /// 考勤详情页面
        /// </summary>
        public ActionResult AttDetail(long attId)
        {
            ViewBag.AttRecordId = attId;
            var attDetailModel = Service.GetStuAttDatail(attId);
            return View("Index_AttDetail", attDetailModel);
        }

        /// <summary>
        /// 获取图形验证码
        /// </summary>
        public ActionResult GetCheckCodeImg()
        {
            var code = "";
            byte[] imgArr = ImgHelper.CreateImgStream(out code);
            SetSession(ComConst.Session_ImgCode, code);
            return File(imgArr, "image/jpge");
        }
        /// <summary>
        /// 获取考勤照片
        /// </summary>
        public ActionResult GetStuAttImg(long attId)
        {
            string imgPath = Service.GetStuAttImgPath(attId);
            if (imgPath.Contains("AttImgs"))
                return File(imgPath, "image/jpge");
            else
                return Content("");
        }

        /// <summary>
        /// 开始绑定关联
        /// </summary>
        public JsonResult DoWxBind(string name, string cardNo, string imgCode)
        {
            var res = "";
            ModelWxUserInfo modelWxUser = JsonHelper.JsonToT<ModelWxUserInfo>(GetCookie(ComConst.Wx_ModelWxUserInfo));
            modelWxUser = new ModelWxUserInfo();
            var imgCodeSession = GetSession(ComConst.Session_ImgCode);
            if (null != modelWxUser && null != imgCodeSession && imgCodeSession.ToUpper().Equals(imgCode.ToUpper()))
                res = Service.DoWxBind(name, cardNo, modelWxUser);
            if (res.Contains("OK"))
            {
                mjResult.code = 1;
                mjResult.content = "ok";
            }
            else
            {
                mjResult.errMsg = res;
            }
            return Json(mjResult);
        }
        /// <summary>
        /// 解除关联
        /// </summary>
        public JsonResult DelWxBinded(byte type,long id)
        {
            var res = Service.DelWxBinded(type, id);
            mjResult.code = (byte)res;
            return Json(mjResult);
        }

        #region 家长绑定及注册相关（弃用）
        public JsonResult GetStudentByName(string stuName, string schoolName)
        {
            var res = Service.GetStudentByName(stuName, schoolName);
            if (res.Length > 6)
            {
                mjResult.code = 1;
                mjResult.content = res;
            }
            else
            {
                mjResult.errMsg = "未查询到您的宝宝，请重试";
            }
            return Json(mjResult);
        }

        public JsonResult BindAndRegisterParent(string stusJson, string parentJson, string imgCode)
        {
            var res = 0;
            ModelWxUserInfo modelWxUser = JsonHelper.JsonToT<ModelWxUserInfo>(GetCookie(ComConst.Wx_ModelWxUserInfo));
            var imgCodeSession = GetSession(ComConst.Session_ImgCode);
            if (null != modelWxUser && null != imgCodeSession && imgCodeSession.ToUpper().Equals(imgCode.ToUpper()))
                res = Service.BindAndRegisterParent(stusJson, parentJson, modelWxUser);
            if (res > 0)
            {
                mjResult.code = 1;
                mjResult.content = "ok";
            }
            else
            {
                mjResult.errMsg = "绑定失败";
            }
            return Json(mjResult);
        }
        #endregion
 
        #region 职工绑定及注册相关（弃用）
        public JsonResult GetStaffByInfo(string staffInfo, string schoolName)
        {
            var res = Service.GetStaffByInfo(staffInfo, schoolName);
            if (res.Length > 6)
            {
                mjResult.code = 1;
                mjResult.content = res;
            }
            else
            {
                mjResult.errMsg = "未查询到您的信息，请重试";
            }
            return Json(mjResult);
        }

        public JsonResult BindOrRegisterStaff(int staffId, string staffJson, string imgCode)
        {
            var res = 0;
            ModelWxUserInfo modelWxUser = JsonHelper.JsonToT<ModelWxUserInfo>(GetCookie(ComConst.Wx_ModelWxUserInfo));
            modelWxUser = new ModelWxUserInfo();
            var imgCodeSession = GetSession(ComConst.Session_ImgCode);
            if (null != modelWxUser && null != imgCodeSession && imgCodeSession.ToUpper().Equals(imgCode.ToUpper()))
                res = Service.BindOrRegisterStaff(staffId, staffJson, modelWxUser);
            if (res > 0)
            {
                mjResult.code = 1;
                mjResult.content = "ok";
            }
            else
            {
                mjResult.errMsg = "绑定失败";
            }
            return Json(mjResult);
        }
        #endregion
    }
}