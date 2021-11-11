using System;
using System.Threading.Tasks;
using Dapr.Client;
using EPlusActivities.API.Infrastructure.ActionResults;
using EPlusActivities.API.Infrastructure.Exceptions;
using Grpc.Core;
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
                    apiResult.StatusCode = StatusCodes.Status503ServiceUnavailable;
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
                case InvocationException ex:
                    switch (ex.InnerException)
                    {
                        case RpcException rpcEx:
                            switch (rpcEx.StatusCode)
                            {
                                case StatusCode.NotFound:
                                    apiResult.StatusCode = StatusCodes.Status404NotFound;
                                    apiResult.Errors = string.IsNullOrEmpty(ex.Message)
                                        ? "Could not find the entity."
                                        : rpcEx.Message;
                                    break;
                                default:
                                    break;
                            }
                            break;
                        default:
                            apiResult.StatusCode = StatusCodes.Status503ServiceUnavailable;
                            apiResult.Errors = string.IsNullOrEmpty(ex.Message)
                                ? "Remote service is unavailbale."
                                : ex.Message;
                            break;
                    }
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
