using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreAPIs.Common;

namespace CoreAPIs.Filters
{
    public class ExceptionFilter  : Attribute, IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            Console.WriteLine("error");
            var actionName = context.HttpContext.Request.RouteValues["controller"] + "/" + context.HttpContext.Request.RouteValues["action"];
            LogHelper.Error($" {actionName} has Error ,Detail:" + context.Exception.Message);
        }
    }

    /*
    if (!context.ExceptionHandled)
    {
        context.Result = new JsonResult(new
        {
            status = false,
            msg = context.Exception.Message
        });
        //context.ExceptionHandled = true;
    }
    */
}


