using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DataService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace PinkOneSysCore.Controllers
{
    [SchoolLoginFilter]
    public class HomeController : BaseController<ISchoolMngService>
    {
        private IMemoryCache _mc;
        public HomeController(IMemoryCache mc)
        {
            _mc = mc;//缓存
        }
        //学校用户首页
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public JsonResult GetSchoolData()
        {
            var res = Service.GetSchoolData();
            if (res.Length > 6)
            {
                mjResult.code = 1;
                mjResult.content = res;
            }
            else
            {
                mjResult.errMsg = "请添加数据";
            }
            return Json(mjResult);
        }


        /// <summary>
        /// 获取登录信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetUserInfo()
        {
            var key = "schoolInfo" + mlUser.School.ID;
            var res = Utility.MemoryCacheHelper.GetCache<string>(key);
            if (res == null || res == "")
                res = Service.GetUserInfo(mlUser.School.ID);
           
            Utility.MemoryCacheHelper.SetCache(key, res, 5);
            mjResult.code = 1;
            mjResult.content = res;

            return Json(mjResult);
        }

        public JsonResult Test()
        {
            var baseDir = System.AppDomain.CurrentDomain.BaseDirectory;
            System.Drawing.Image img = System.Drawing.Image.FromFile(baseDir + "\\TempFiles\\test2.jpg");
            //System.Drawing.Bitmap b= Utility.ImgHelper.ImageCut(img);
            //b.Save(baseDir + "\\TempFiles\\test2_1.jpg");
            Utility.ImgHelper.ImageCompress(img, baseDir + "\\TempFiles\\test2_2.jpg");
            //img.Clone();
            img.Dispose();
            //Utility.ImgHelper.ImageCompress(b, baseDir + "\\TempFiles\\test2_3.jpg");
            return Json(mjResult);
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();//View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
