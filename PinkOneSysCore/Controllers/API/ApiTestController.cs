using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace PinkOneSysCore.Controllers.API
{
    public class ApiTestController : Controller
    {
        // GET: ApiTest
        public ActionResult Index()
        {
            return View("Index_ApiTest");
        }
    }
}