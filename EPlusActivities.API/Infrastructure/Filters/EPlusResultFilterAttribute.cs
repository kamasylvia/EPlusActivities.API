using System.Threading.Tasks;
using EPlusActivities.API.Infrastructure.ActionResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EPlusActivities.API.Infrastructure.Filters
{
    public class EPlusResultFilterAttribute : ResultFilterAttribute, IAsyncAlwaysRunResultFilter
    {
        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            switch (context.Result)
            {
                case ObjectResult objectResult:
                    var apiResult = new ApiResult(objectResult);
                    apiResult.StatusCode = objectResult.StatusCode ?? context.HttpContext.Response.StatusCode;
                    objectResult.Value = apiResult;
                    break;
                case StatusCodeResult statusCodeResult:
                    apiResult = new ApiResult(statusCodeResult);
                    context.Result = new ObjectResult(apiResult);
                    break;
                default:
                    break;
            }

            await next();
        }
    }
}