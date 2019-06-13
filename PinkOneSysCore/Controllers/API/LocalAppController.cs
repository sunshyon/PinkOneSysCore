using DataService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Utility;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace PinkOneSysCore.Controllers.API
{
    public class LocalAppController : ControllerBase
    {
        private string baseFileDir = ConfigHelper.AppSettings("BaseFileDir");
        private string apiWebServer= ConfigHelper.AppSettings("ApiWebServer"); 
        private string fileWebServer= ConfigHelper.AppSettings("FileWebServer");
        public ModelJsonRet mjResult;
        public ILocalAppApiService Service { get; set; }
        public LocalAppController()
        {
            Service = new LocalAppApiService();
            mjResult = new ModelJsonRet()
            {
                code = 0,
                errMsg = string.Empty,
                content = string.Empty
            };
        }

        [HttpPost, Route("api/LocalApp/OAuth")]
        public ModelJsonRet OAuth()
        {
            try
            {
                var form = HttpContext.Request.Form;
                var username = form["username"].ToString().Trim();
                var password = form["password"].ToString().Trim();
                var schoolId = int.Parse(form["schoolId"].ToString().Trim());
                return Service.OAuth(username,password, schoolId);
            }
            catch (Exception e)
            {
                mjResult.errMsg = e.Message;
                return mjResult;
            }
        }
        [HttpPost,Route("api/LocalApp/GetSchoolInfo")]
        public ModelJsonRet GetSchoolInfo()
        {
            try
            {
                var form = HttpContext.Request.Form;
                //var token = request["token"];
                var username = form["username"].ToString().Trim();
                var password = form["password"].ToString().Trim();
                var schoolId = int.Parse(form["schoolId"].ToString().Trim());
                return Service.GetSchoolInfo(username, password, schoolId);
            }
            catch (Exception e)
            {
                mjResult.errMsg = e.Message;
                return mjResult;
            }
        }
        [HttpPost, Route("api/LocalApp/UploadAttendance")]
        public ModelJsonRet UploadAttendance()
        {
            try
            {
                var form = HttpContext.Request.Form;
                //var token = request["token"];
                var schoolId= int.Parse(form["schoolId"].ToString().Trim());
                byte personType = byte.Parse(form["personType"].ToString().Trim());
                long personId = long.Parse(form["personId"].ToString().Trim());
                byte attWay = byte.Parse(form["attWay"].ToString().Trim());
                var cardNo = form["cardNo"].ToString().Trim();
                var dateTime = form["dateTime"].ToString().Trim();
                var deviceId= form["deviceId"].ToString().Trim();
               
                long attId = 0;
                mjResult= Service.UploadAtt(schoolId, personType, personId, attWay, cardNo, dateTime, deviceId,out attId);
                if (mjResult.code == 1 && attId > 0)
                {
                    Task t = new Task(() =>
                    {
                        var fileUrl = "";
                        //查找对应考勤图片是否存在
                        var isExist_url = apiWebServer + "api/FileApi/IsFileExist";
                        var realtivePath = schoolId + "//AttImgs//" + DateTime.Now.ToString("yyyyMM") + "//" + personId + "//";
                        var fullDic = baseFileDir + realtivePath;
                        var filename = personId + "_" + dateTime + ".jpg";
                        var filefullname = fullDic + filename;
                        var json = new
                        {
                            filefullname = filefullname
                        };
                        var res = HttpService.PostUrl(isExist_url, JsonHelper.ToJson(json));
                        var postRes = JsonHelper.JsonToT<ModelJsonRet>(res);
                        if (postRes.code == 1)//图片存在
                        {
                            fileUrl = fileWebServer + realtivePath + filename;
                            //byte[] tmp = System.Text.Encoding.ASCII.GetBytes(fileUrl);
                            //fileUrl = Convert.ToBase64String(tmp);
                            //更新考勤图片地址
                            Service.UpdateAttImg(attId, fileUrl);
                        }
                    });
                    t.Start();
                }

                return mjResult;
            }
            catch(Exception e)
            {
                mjResult.errMsg = e.Message;
                return mjResult;
            }
        }
        [HttpPost,Route("api/LocalApp/UploadAttImg")]
        public async Task<ModelJsonRet> UploadAttImg()
        {
            try
            {
                await Task.Delay(10);
                var form = HttpContext.Request.Form;
                //var token = request["token"];
                var schoolId = int.Parse(form["schoolId"].ToString().Trim());
                long personId = long.Parse(form["personId"].ToString().Trim());
                var dateTime = form["dateTime"].ToString().Trim();

                if (form.Files.Count > 0)
                {
                    var file = form.Files[0];
                    if (file.FileName.IndexOf("jpg") < 0 || dateTime.Length != 17)
                    {
                        mjResult.errMsg = "为了方便系统匹配，图片名应统一为.jpg，并且dateTime格式为字符串：yyyyMMddHHmmssfff";
                        return mjResult;
                    }

                    //上传图片文件
                    var url = apiWebServer + "api/FileApi/UploadFile";
                    var tmpfiledir = AppDomain.CurrentDomain.BaseDirectory+"TempFiles//";
                    var filename = personId + "_" + dateTime + ".jpg";
                    var tmpFullName = tmpfiledir + filename;
                    if (!Directory.Exists(tmpfiledir))
                        Directory.CreateDirectory(tmpfiledir);
                    //file.SaveAs(tmpFullName);

                    //裁剪并压缩
                    var img = System.Drawing.Image.FromStream(file.OpenReadStream());
                    ImgHelper.ImageCompress(img, tmpFullName);
                    img.Dispose();
                    img = null;

                    var realtivePath= schoolId + "//AttImgs//" + DateTime.Now.ToString("yyyyMM") + "//" + personId + "//";
                    var fullPhysicalPath = baseFileDir + realtivePath;
                    fullPhysicalPath = System.Web.HttpUtility.UrlEncode(fullPhysicalPath);

                    var postData = new
                    {
                        filename = filename,
                        filepath = fullPhysicalPath
                    };
                    var res = HttpService.UpLoadFileWithParam(url, JsonHelper.ToJson(postData), tmpFullName);
                    mjResult = JsonHelper.JsonToT<ModelJsonRet>(res);
                    if (mjResult.code == 1)
                    {
                       
                        if (System.IO.File.Exists(tmpFullName))
                            System.IO.File.Delete(tmpFullName);

                        //文件上传成功，修改数据库
                        Task t = new Task(() =>
                        {
                            long attId = 0;
                            if (Service.HasRelatedAttRecord(schoolId, personId, dateTime, out attId))//考勤记录存在
                            {
                                var fileUrl = fileWebServer + realtivePath + filename;
                                //byte[] tmp = System.Text.Encoding.ASCII.GetBytes(fileUrl);
                                //fileUrl = Convert.ToBase64String(tmp);
                                //更新考勤图片地址
                                Service.UpdateAttImg(attId, fileUrl);
                            }
                        });
                        t.Start();
                    }
                    return mjResult;
                   
                }
                mjResult.errMsg = "无图片文件";
                return mjResult;
            }
            catch (Exception e)
            {
                mjResult.errMsg = e.Message;
                return mjResult;
            }
        }
    }
}
