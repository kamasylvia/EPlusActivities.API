using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Infrastructure.ActionResults;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace EPlusActivities.API.Infrastructure.Filters
{
    public class CustomExceptionFilter : IAsyncExceptionFilter
    {
        private readonly ILogger<CustomExceptionFilter> _logger;

        public CustomExceptionFilter(ILogger<CustomExceptionFilter> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            string methodInfo =
                $"{context.RouteData.Values["controller"] as string}Controller.{context.RouteData.Values["action"] as string}:{context.HttpContext.Request.Method}";

            _logger.LogError(context.Exception, "执行{0}时发生错误！", methodInfo);
            context.Result = new InternalServerErrorObjectResult("Server Error");
        }
    }
}
