using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Utility;

namespace PinkOneSysCore.Controllers.API
{
    public class FileApiController : ControllerBase
    {
        public ModelJsonRet mjResult;
        public FileApiController()
        {
            mjResult = new ModelJsonRet()
            {
                code = 0,
                errMsg = string.Empty,
                content = string.Empty
            };
        }

        [HttpPost, Route("api/FileApi/UploadFile")]
        public async Task<ModelJsonRet> UploadFile()
        {
            try
            {
                var form = HttpContext.Request.Form;
                var name = form["filename"].ToString().Trim();
                var path = form["filepath"].ToString().Trim();
                path =HttpUtility.UrlDecode(path);
                if (form.Files.Count > 0)
                {
                    var file = form.Files[0];
                    var fullpathname = path + name;
                    if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                    using (var stream = new FileStream(fullpathname, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                       
                    mjResult.code = 1;
                    mjResult.content = "OK";
                    return mjResult;
                }
                mjResult.content = "无文件";
                return mjResult;
            }
            catch (Exception e)
            {
                mjResult.errMsg = e.Message;
                return mjResult;
            }
        }
        /// <summary>
        /// 传递json
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("api/FileApi/IsFileExist")]
        public ModelJsonRet IsFileExist([FromBody] JObject json)
        {
            try
            {
                var filefullname = json["filefullname"].ToString().Trim();
                filefullname = HttpUtility.UrlDecode(filefullname);
                if (System.IO.File.Exists(filefullname))
                {
                    mjResult.code = 1;
                }
                
                return mjResult;
            }
            catch (Exception e)
            {
                mjResult.errMsg = e.Message;
                return mjResult;
            }
        }

        [HttpPost, Route("api/FileApi/DelFile")]
        public ModelJsonRet DelFile([FromBody] JObject json)
        {
            try
            {
                var filefullname = json["filefullname"].ToString().Trim();
                filefullname = HttpUtility.UrlDecode(filefullname);
                if (System.IO.File.Exists(filefullname))
                {
                    System.IO.File.Delete(filefullname);
                    mjResult.code = 1;
                }
                return mjResult;
            }
            catch (Exception e)
            {
                mjResult.errMsg = e.Message;
                return mjResult;
            }
        }
        /// <summary>
        /// 删除文件夹
        /// </summary>
        [HttpPost, Route("api/FileApi/DelFolder")]
        public ModelJsonRet DelFolder([FromBody] JObject json)
        {
            try
            {
                var folderPath = json["folderPath"].ToString().Trim();
                folderPath = HttpUtility.UrlDecode(folderPath);
                if (Directory.Exists(folderPath))
                {
                    Directory.Delete(folderPath);
                    mjResult.code = 1;
                }
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