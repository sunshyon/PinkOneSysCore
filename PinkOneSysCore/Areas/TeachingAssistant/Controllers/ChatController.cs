using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataService;
using Microsoft.AspNetCore.Mvc;

namespace PinkOneSysCore.Areas.TeachingAssistant.Controllers
{
    [Area("TeachingAssistant")]
    [SchoolLoginFilter]
    public class ChatController : BaseController<IAssistantService>
    {
        public IActionResult Index()
        {
            return View("Index_Chat");
        }

        public JsonResult GetChatInfo()
        {
            mjResult= Service.GetChatInfo();
            return Json(mjResult);
        }

    }
}