using System.Threading.Tasks;
using EPlusActivities.API.Infrastructure.ActionResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EPlusActivities.API.Infrastructure.Filters
{
    public class CustomActionFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnResultExecutionAsync(
            ResultExecutingContext context,
            ResultExecutionDelegate next
        )
        {
            switch (context.Result)
            {
                case ObjectResult objectResult:
                    objectResult.Value = new ApiResult
                    {
                        StatusCode = objectResult.StatusCode,
                        Data = objectResult.Value,
                        Succeeded = true
                    };
                    break;
                default:
                    break;
            }
            await next();
        }
    }
}
