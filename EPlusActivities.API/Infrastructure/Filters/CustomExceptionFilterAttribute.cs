using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace EPlusActivities.API.Infrastructure.Filters
{
    public class CustomExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly ILogger<CustomExceptionFilterAttribute> _logger;

        public CustomExceptionFilterAttribute(ILogger<CustomExceptionFilterAttribute> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            string methodInfo =
                $"{context.RouteData.Values["controller"] as string}Controller.{context.RouteData.Values["action"] as string}:{context.HttpContext.Request.Method}";

            _logger.LogError(context.Exception, "执行{0}时发生错误！", methodInfo);

            var apiResult = new ApiResult { Succeeded = false };
            switch (context.Exception)
            {
                case BadRequestException ex:
                    apiResult.StatusCode = StatusCodes.Status400BadRequest;
                    apiResult.Errors = string.IsNullOrEmpty(ex.Message)
                        ? "Bad request error."
                        : ex.Message;
                    break;
                case NotFoundException ex:
                    apiResult.StatusCode = StatusCodes.Status404NotFound;
                    apiResult.Errors = string.IsNullOrEmpty(ex.Message)
                        ? "Could not find the entity."
                        : ex.Message;
                    break;
                case DatabaseUpdateException ex:
                    apiResult.StatusCode = StatusCodes.Status500InternalServerError;
                    apiResult.Errors = string.IsNullOrEmpty(ex.Message)
                        ? "Database Update Error."
                        : ex.Message;
                    break;
                case RemoteServiceException ex:
                    apiResult.StatusCode = StatusCodes.Status502BadGateway;
                    apiResult.Errors = string.IsNullOrEmpty(ex.Message)
                        ? "Remote service error."
                        : ex.Message;
                    break;
                case ConflictException ex:
                    apiResult.StatusCode = StatusCodes.Status409Conflict;
                    apiResult.Errors = string.IsNullOrEmpty(ex.Message)
                        ? "Conflict error."
                        : ex.Message;
                    break;
                default:
                    apiResult.StatusCode = StatusCodes.Status500InternalServerError;
                    apiResult.Errors = "Internal Server Error.";
                    break;
            }

            context.Result = new ObjectResult(apiResult) { StatusCode = apiResult.StatusCode };
        }
    }
}
