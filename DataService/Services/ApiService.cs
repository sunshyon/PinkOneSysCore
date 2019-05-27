using DataAccess;
using Domain;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace DataService
{
    public class ApiService:IApiService
    {
        #region 基础部分
        private static IUnitOfWork _unitOfWork;
        public ModelWxSetting mWxSetting = JsonFileProvider.Instance.GetSettings<ModelWxSetting>();
        public string baseFileDir = ConfigHelper.AppSettings("BaseFileDir");
        private string fileWebServer = ConfigHelper.AppSettings("FileWebServer");
        /// <summary>
        /// 获取数据单元模块
        /// </summary>
        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (_unitOfWork == null)
                {
                    _unitOfWork = new UnitOfWork();
                }
                return _unitOfWork;
            }
        }
        private ModelJsonRet _mjr; 
        /// <summary>
        /// 构造函数
        /// </summary>
        public ApiService()
        {
            _mjr = new ModelJsonRet()
            {
                code = 0,
                content = "",
                errMsg = ""
            };
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        public void Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion

        #region 考勤API服务
        /// <summary>
        /// 判断是否有相关联的考勤记录
        /// </summary>
        /// <param name="dateTimeStr">yyyyMMddHHmmssfff</param>
        /// <returns></returns>
        public bool HasRelatedAttRecord(int schoolId, long stuId, string dateTimeStr ,out long attId)
        {
            attId = 0;
            var school = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID == schoolId && x.Status == 1).Result.FirstOrDefault();
            if (school == null)
                return false;
            var stuObj = UnitOfWork.Repository<SYS_Student>().GetEntitiesAsync(x => x.SchoolId == schoolId && x.ID == stuId).Result.FirstOrDefault();
            if (null == stuObj)
                return false;

            var todayStart = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));
            var relatedAttRecord = UnitOfWork.Repository<SYS_StudentAttRecord>().GetEntitiesAsync(x => x.AttTime > todayStart && x.AttTimeStr == dateTimeStr && x.SchoolId == schoolId && x.MasterId == stuId).Result.FirstOrDefault();
            if (null != relatedAttRecord)
            {
                attId = relatedAttRecord.ID;
                return true;
            }
            return false;
        }
        /// <summary>
        /// 判断并返回图片地址文件夹路径
        /// dateTimeStr :yyyyMMddHHmmssfff
        /// </summary>
        public string UpdateAttImg(long id,string fileUrl)
        {
            var relatedAttRecord = UnitOfWork.Repository<SYS_StudentAttRecord>().GetEntitiesAsync(x =>x.ID==id).Result.FirstOrDefault();
            if (null!=relatedAttRecord)
            {
                relatedAttRecord.MonitoringImg = fileUrl;
                UnitOfWork.Repository<SYS_StudentAttRecord>().UpdateEntity(relatedAttRecord);
                if(UnitOfWork.CommitAsync().Result>0)
                    return "OK";
            }
            return "失败：数据库异常！";
        }

        /// <summary>
        /// 上传考勤数据
        /// </summary>
        /// <param name="token"></param>
        /// <param name="personType">人员类型1：学生，2：职工</param>
        /// <param name="attWay">考勤方式1：刷卡，2：二维码</param>
        public ModelJsonRet UploadAtt(int schoolId, byte personType, long personId,byte attWay, string cardNo,string dateTime,string deviceId,out long attId)
        {
            attId = 0;
            var res = 0;
            //byte[] byteArray = Convert.FromBase64String(token);
            //string tokenStr= Encoding.Default.GetString(byteArray);
            //var schoolId = int.Parse(tokenStr.Split(',')[0]);
            var school = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID == schoolId && x.Status == 1).Result.FirstOrDefault();
            if (school == null)
            {
                _mjr.errMsg = "验证失败或学校id不存在或该学校已注销";
                return _mjr;
            }
            var attTime = DateTime.ParseExact(dateTime, "yyyyMdHHmmssfff", System.Globalization.CultureInfo.InvariantCulture); 
            var todayStart = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd 00:00:00"));

            if (personType == 1)//学生考勤
            {
                var stuObj = UnitOfWork.Repository<SYS_Student>().GetEntitiesAsync(x => x.SchoolId == schoolId && x.ID == personId).Result.FirstOrDefault();
                if (null == stuObj)
                {
                    _mjr.errMsg = "未找到学生ID";
                    return _mjr;
                }
                var attType = AttType.签入;
                var currentAttStatus = CurrentAttStatus.在校;
                //判断是签入还是签出
                var todayAttRecords = UnitOfWork.Repository<SYS_StudentAttRecord>().GetEntitiesAsync(x => x.AttTime > todayStart).Result;
                if (todayAttRecords.Count > 0 && todayAttRecords.Count % 2 == 1)
                {
                    attType = AttType.签出;
                    currentAttStatus = CurrentAttStatus.离校;
                }

                SYS_StudentAttRecord stuAttRecord = new SYS_StudentAttRecord
                {
                    SchoolId = school.ID,
                    MasterId = personId,
                    AttWay = attWay,
                    AttType = (byte)attType,
                    AttTime = attTime,
                    AttTimeStr = dateTime,
                };
                UnitOfWork.Repository<SYS_StudentAttRecord>().AddEntity(stuAttRecord);
                var addOk = UnitOfWork.CommitAsync().Result;
                if (addOk > 0)
                {
                    res = addOk;
                    attId = stuAttRecord.ID;
                    //更新学生当前考勤状态
                    stuObj.AttStatus = (byte)currentAttStatus;
                    UnitOfWork.Repository<SYS_Student>().UpdateEntity(stuObj);

                    //推送考勤消息
                    List<FK_Stu_Parent> fks = UnitOfWork.Repository<FK_Stu_Parent>().GetEntitiesAsync(x => x.SchoolId == stuObj.SchoolId && x.StuId == stuObj.ID).Result;//查找学生家长
                    if (null != fks && fks.Count > 0)
                    {
                        var pWxPInfo = UnitOfWork.Repository<Wx_PublicInfo>().GetEntitiesAsync(x => x.Type == 1).Result.FirstOrDefault();
                        var wxpInfoId = pWxPInfo==null?0: pWxPInfo.ID;
                        if (school.WxPublicInfoId != null && school.WxPublicInfoId > 0)
                            wxpInfoId = (int)school.WxPublicInfoId;
                        var wxPubInfo = UnitOfWork.Repository<Wx_PublicInfo>().GetEntitiesAsync(x => x.ID== wxpInfoId).Result.FirstOrDefault();
                        foreach (var fk in fks)
                        {
                            //推送消息
                            if (null != wxPubInfo)
                            {
                                // 需要从Cookie中获取
                                ModelWxMsg<ModelWmAttendance> wxMsg = new ModelWxMsg<ModelWmAttendance>();
                                wxMsg = JsonFileProvider.Instance.GetSettings<ModelWxMsg<ModelWmAttendance>>();
                                wxMsg.data.first.value = string.Format(wxMsg.data.first.value, (AttType)attType);
                                wxMsg.url = mWxSetting.PubUrl_Host + "/WxRelated/Center/AttDetail?attId=" + stuAttRecord.ID;
                                wxMsg.touser = fk.OpenId;
                                wxMsg.data.keyword1.value = stuObj.StuName;
                                wxMsg.data.keyword2.value = attTime.ToString();
                                wxMsg.data.keyword3.value = school.SchoolName;
                                ModelWmResult wmResult = WXOAuthApiHelper.SendTmplMessage(wxPubInfo.AccessToken, wxMsg);
                                if (null == wmResult || wmResult.msgid <= 0)
                                {
                                    LogHelper.Error("推送错误:" + wmResult.errmsg);
                                }
                            }
                        }
                    }
                }
            }
            else if (personType == 2)//职工考勤
            {
                var staffObj = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.SchoolId == schoolId && x.ID == personId).Result.FirstOrDefault();
                if (null == staffObj)
                {
                    _mjr.errMsg = "未找到职工ID";
                    return _mjr;
                }
                var attType = AttType.签入;
                var currentAttStatus = CurrentAttStatus.在校;
                //判断是签入还是签出

                var todayAttRecords = UnitOfWork.Repository<SYS_StaffAttRecord>().GetEntitiesAsync(x => x.AttTime > todayStart).Result.ToList();
                if (todayAttRecords.Count > 0 && todayAttRecords.Count % 2 == 1)
                {
                    attType = AttType.签出;
                    currentAttStatus = CurrentAttStatus.离校;
                }
                SYS_StaffAttRecord stuAttRecord = new SYS_StaffAttRecord
                {
                    SchoolId = school.ID,
                    MasterId = (int)personId,
                    AttWay = attWay,
                    AttType = (byte)attType,
                    AttTime = attTime,
                    AttTimeStr = dateTime,
                };
                UnitOfWork.Repository<SYS_StaffAttRecord>().AddEntity(stuAttRecord);
                var addOk = UnitOfWork.CommitAsync().Result;
                if (addOk > 0)
                {
                    res = addOk;
                    attId = stuAttRecord.ID;
                    //更新当前考勤状态
                    staffObj.AttStatus = (byte)currentAttStatus;
                    UnitOfWork.Repository<SYS_Staff>().UpdateEntity(staffObj);

                    //推送考勤消息
                    var pWxPInfo = UnitOfWork.Repository<Wx_PublicInfo>().GetEntitiesAsync(x => x.Type == 1).Result.FirstOrDefault();
                    var wxpInfoId = pWxPInfo == null ? 0 : pWxPInfo.ID;
                    if (school.WxPublicInfoId != null && school.WxPublicInfoId > 0)
                        wxpInfoId = (int)school.WxPublicInfoId;
                    var wxPubInfo = UnitOfWork.Repository<Wx_PublicInfo>().GetEntitiesAsync(x => x.ID == wxpInfoId).Result.FirstOrDefault();
                    if (null != wxPubInfo && staffObj.OpenId != null && staffObj.OpenId.Length > 6)
                    {
                        // 需要从Cookie中获取
                        ModelWxMsg<ModelWmAttendance> wxMsg = new ModelWxMsg<ModelWmAttendance>();
                        wxMsg = JsonFileProvider.Instance.GetSettings<ModelWxMsg<ModelWmAttendance>>();
                        wxMsg.data.first.value = (AttType)attType + " 成功";
                        //wxMsg.url = mWxSetting.PubUrl_Host + "/WxRelated/Center/AttDetail?attId=" + stuAttRecord.ID;
                        wxMsg.touser = staffObj.OpenId;
                        wxMsg.data.keyword1.value = staffObj.StaffName;
                        wxMsg.data.keyword2.value = attTime.ToString();
                        wxMsg.data.keyword3.value = school.SchoolName;
                        ModelWmResult wmResult = WXOAuthApiHelper.SendTmplMessage(wxPubInfo.AccessToken, wxMsg);
                        if (null == wmResult || wmResult.msgid <= 0)
                        {
                            LogHelper.Error("推送错误:" + wmResult.errmsg);
                        }

                    }
                }
            }
            if (res > 0)
            {
                _mjr.code = 1;
                _mjr.content = "ok";
                //RefreshEntities("stu");
                //RefreshEntities("staff");
            }
            return _mjr;
        }

        public ModelJsonRet OAuth(string username,string password,int schoolId)
        {
            var admin = UnitOfWork.Repository<SYS_Admin>().GetEntitiesAsync(x => x.Username == username && x.Password == password).Result.FirstOrDefault();
            var school = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID==schoolId).Result.FirstOrDefault();
            if (null!= admin && null!=school )
            {
                var tokenStr = school.ID+ "," + Guid.NewGuid().ToString();
                byte[] byteArray = System.Text.Encoding.Default.GetBytes(tokenStr);
                var token = Convert.ToBase64String(byteArray);
                school.Token = token;
                UnitOfWork.Repository<SYS_School>().UpdateEntity(school);
                var isOk = UnitOfWork.CommitAsync().Result;
                if (isOk>0)
                {
                    var json = new
                    {
                        SchoolId = school.ID,
                        SchoolName = school.SchoolName,
                        Token = token
                    };
                    _mjr.code = 1;
                    _mjr.content = json;
                    return _mjr;
                }
            }
            else
            {
                _mjr.errMsg = "账号或密码错误";
            }
            return _mjr;
        }

        public ModelJsonRet GetSchoolInfo(string username,string password, int schoolId,string token="")
        {
            var admin = UnitOfWork.Repository<SYS_Admin>().GetEntitiesAsync(x => x.Username == username && x.Password == password).Result.FirstOrDefault();
            var school = UnitOfWork.Repository<SYS_School>().GetEntitiesAsync(x => x.ID == schoolId&&x.Status==1).Result.FirstOrDefault();
            if (admin==null||school == null)
            {
                _mjr.errMsg = "验证失败或学校id不存在或该学校已注销";
                return _mjr;
            }
            var stus = UnitOfWork.Repository<SYS_Student>().GetEntitiesAsync(x => x.SchoolId == school.ID && x.Status == (byte)StuStatus.正常).Result.ToList();
            var staffs = UnitOfWork.Repository<SYS_Staff>().GetEntitiesAsync(x => x.SchoolId == school.ID && x.Status == (byte)StaffStatus.在职).Result.ToList();
            var cards = UnitOfWork.Repository<SYS_Card>().GetEntitiesAsync(x => x.SchoolId == school.ID && x.Status == (byte)CardStatus.正常).Result.ToList();

            //学生
            List<ModelApiStu> mastulist = new List<ModelApiStu>();
            foreach (var s in stus)
            {
                
                var sCardList = cards.Where(x => x.CardMasterId == s.ID&&x.CardType==(byte)CardType.学生卡).ToList();
                List<string> cardList = new List<string>();
                foreach (var c in sCardList)
                {
                    cardList.Add(c.CardNo);
                }
                mastulist.Add(new ModelApiStu()
                {
                    StuId=s.ID,
                    StuName=s.StuName,
                    CardList= cardList
                });
            }
            //职员
            List<ModelApiStaff> mastafflist = new List<ModelApiStaff>();
            foreach (var s in staffs)
            {
                var sCardList = cards.Where(x => x.CardMasterId == s.ID && x.CardType == (byte)CardType.职员卡).ToList();
                List<string> cardList = new List<string>();
                foreach (var c in sCardList)
                {
                    cardList.Add(c.CardNo);
                }
                mastafflist.Add(new ModelApiStaff()
                {
                    StaffId = s.ID,
                    StaffName = s.StaffName,
                    CardList = cardList
                });
            }
            var json = new
            {
                SchoolId = school.ID,
                SchoolName = school.SchoolName,
                Students = mastulist,
                Staffs= mastafflist
            };

            _mjr.code = 1;
            _mjr.content = json;
            return _mjr;
        }
        #endregion

        #region 文件API服务
        /// <summary>
        /// 上传文件
        /// </summary>
        public bool UploadFile(string name,string path)
        {
            return true;
        }
        /// <summary>
        /// 删除文件
        /// </summary>
        public bool DeleteFile(string fullpath)
        {
            return false;
        }
        #endregion

    }
}
