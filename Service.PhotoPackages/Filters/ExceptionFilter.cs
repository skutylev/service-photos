using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Service.PhotoPackages.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute

    {
        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.Exception is ArgumentException ||
                context.Exception is ArgumentNullException ||
                context.Exception is ArgumentOutOfRangeException ||
                context.Exception is BadHttpRequestException
            )
            {
                context.Result = new BadRequestObjectResult(new {Error = context.Exception.Message});
            }

            await base.OnExceptionAsync(context);
        }
    }
}