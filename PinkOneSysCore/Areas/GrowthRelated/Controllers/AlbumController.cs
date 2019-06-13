using DataService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Utility;

namespace PinkOneSysCore.Areas.GrowthRelated.Controllers
{
    [Area("GrowthRelated")]
    [SchoolLoginFilter]
    public class AlbumController : BaseController<IGrowthService>
    {
        private string baseFileDir = ConfigHelper.AppSettings("BaseFileDir");
        private string apiWebServer = ConfigHelper.AppSettings("ApiWebServer");
        private string fileWebServer = ConfigHelper.AppSettings("FileWebServer");
        // GET: GrowthRelated/Album
        public ActionResult Index()
        {
            return View("Index_Album");
        }
        public ActionResult AlbumDetail(long aId)
        {
            ViewBag.AlbumId = aId;
            return View("Index_AlbumDetail");
        }

        [HttpGet]
        public JsonResult GetAlbumsInfo(string nQuery, int cQuery,int pageIndex)
        {
            mjResult = Service.GetAlbumsInfo(nQuery, cQuery, pageIndex);
            
            return Json(mjResult);
        }
        [HttpGet]
        public JsonResult GetNoAlbumStus()
        {
            mjResult = Service.GetNoAlbumStus();

            return Json(mjResult);
        }
        [HttpPost]
        public JsonResult AddSingleAlbum(string name,byte type,int classId,long stuId)
        {
            mjResult = Service.AddSingleAlbum(name,type,classId,stuId); 

            return Json(mjResult);
        }
        [HttpPost]
        public JsonResult AutoAddStuAlbum(int classId)
        {
            mjResult = Service.AutoAddStuAlbum(classId); 

            return Json(mjResult);
        }
        [HttpPost]
        public JsonResult DelAlbum(long aId)
        {
            var isOK = true;// Service.DelAlbum(aId);
            if (isOK)
            {
                //删除文件夹
                var del_url = apiWebServer + "api/FileApi/DelFolder";
                var realtivePath = mlUser.School.ID + "//Photos//" + aId + "//";
                var folderPath = baseFileDir + realtivePath;
                var json = new
                {
                    folderPath = folderPath
                };
                var del_res = HttpService.PostUrl(del_url, JsonHelper.ToJson(json));
                var postRes = JsonHelper.JsonToT<ModelJsonRet>(del_res);
                if (postRes.code == 1)
                {
                    mjResult = postRes;
                }
                else
                {
                    mjResult.code = 1;
                    LogHelper.Info("DelAlbum：相册数据已删除，但是文件夹删除失败！");
                }
            }
            else
                mjResult.errMsg = "删除失败";
            return Json(mjResult);
        }

        [HttpGet]
        public JsonResult GetPhotosInfo(long aId,int pageIndex)
        {
            mjResult = Service.GetPhotosInfo(aId, pageIndex);

            return Json(mjResult);
        }
        [HttpPost]
        public async Task<JsonResult> UploadPhoto()
        {
            await Task.Delay(10);
            var form = HttpContext.Request.Form;
            var describe = form["describe"].ToString();
            var albumId = long.Parse(form["aId"].ToString());
            var schoolId = mlUser.School.ID;

            var fileCount = form.Files.Count;
            if (fileCount > 0)
            {
                var files = form.Files;
                var okCount = 0;
                for (int i = 0; i < fileCount; i++)
                {
                    var file = files[i];
                    //上传图片文件
                    var url = apiWebServer + "api/FileApi/UploadFile";
                    var tmpfiledir = AppDomain.CurrentDomain.BaseDirectory + "TempFiles\\";
                    var filename = albumId + "_" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".jpg";
                    var tmpFullName = tmpfiledir + filename;
                    if (!Directory.Exists(tmpfiledir))
                        Directory.CreateDirectory(tmpfiledir);
                    //file.SaveAs(tmpFullName);

                    //裁剪并压缩
                    var img = System.Drawing.Image.FromStream(file.OpenReadStream());
                    ImgHelper.ImageCompress(img, tmpFullName);
                    var sizeStr = img.Width + "*" + img.Height;
                    img.Dispose();
                    img = null;

                    var realtivePath = schoolId + "//Photos//" + albumId + "//";
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
                        var fileUrl = fileWebServer + realtivePath + filename;
                        var isOK = Service.AddPhotoRecord(albumId, describe, fileUrl, sizeStr);
                        if (isOK > 0)
                            okCount++;
                        else
                        {
                            //删除文件
                            var del_url = apiWebServer + "api/FileApi/DelFile";
                            var fullDic = baseFileDir + realtivePath;
                            var filefullname = fullPhysicalPath + filename;
                            var json = new
                            {
                                filefullname = filefullname
                            };
                            var del_res = HttpService.PostUrl(del_url, JsonHelper.ToJson(json));
                            var postRes = JsonHelper.JsonToT<ModelJsonRet>(del_res);
                        }
                    }
                }
                if (okCount > 0 && form.Files.Count == okCount)
                {
                    mjResult.code = 1;
                    mjResult.content = "全部上传成功";
                }
                else if (okCount > 0)
                {
                    mjResult.code = 1;
                    mjResult.content = okCount + "张成功，" + (form.Files.Count - okCount) + "张失败";
                }
            }
            mjResult.errMsg = "无图片文件";
            // }
            //catch (Exception e)
            //{
            //    mjResult.code = 0;
            //    mjResult.errMsg = e.Message;
            //}
            return Json(mjResult);
        }
        [HttpPost]
        public JsonResult DelPhoto(long prId)
        {
            var imgUrl = "";
            var isOK = Service.DelPhotoRecord(prId, out imgUrl);
            if (isOK&& imgUrl.Length>0)
            {
                //删除文件
                var del_url = apiWebServer + "api/FileApi/DelFile";
                var realtivePathName = imgUrl.Substring(fileWebServer.Length);
                var fullPhysicalPathName = baseFileDir + realtivePathName;
                var json = new
                {
                    filefullname = fullPhysicalPathName
                };
                var del_res = HttpService.PostUrl(del_url, JsonHelper.ToJson(json));
                var postRes = JsonHelper.JsonToT<ModelJsonRet>(del_res);
                if (postRes.code == 1)
                {
                    mjResult = postRes;
                }
                else
                {
                    mjResult.code = 1;
                    LogHelper.Info("DelPhoto：照片记录已删除，但是文件删除失败！");
                }
            }
            else
                mjResult.errMsg = "删除失败";
            return Json(mjResult);
        }
    }
}