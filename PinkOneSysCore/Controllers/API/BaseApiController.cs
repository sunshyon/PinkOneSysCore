using DataService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Utility;
using Microsoft.AspNetCore.Mvc;

namespace PinkOneSysCore.Controllers.API
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class BaseApiController: ControllerBase
    {
        public ModelJsonRet mjResult;
        public IApiService Service { get; set; }
        public BaseApiController()
        {
            Service = new ApiService();
            mjResult = new ModelJsonRet()
            {
                code = 0,
                errMsg = string.Empty,
                content = string.Empty
            };
        }
    }
}