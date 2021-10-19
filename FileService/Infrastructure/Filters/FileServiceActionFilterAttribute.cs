using System.Threading.Tasks;
using FileService.Infrastructure.ActionResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FileService.Infrastructure.Filters
{
    public class FileServiceActionFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnResultExecutionAsync(
            ResultExecutingContext context,
            ResultExecutionDelegate next
        )
        {
            switch (context.Result)
            {
                case ObjectResult objectResult:
                    var apiResult = new ApiResult(objectResult);
                    objectResult.Value = apiResult;
                    break;
                case StatusCodeResult statusCodeResult:
                    apiResult = new ApiResult(statusCodeResult);
                    var result = new ObjectResult(apiResult);
                    result.StatusCode = apiResult.StatusCode;
                    context.Result = result;
                    break;
                default:
                    break;
            }
            await next();
        }
    }
}
