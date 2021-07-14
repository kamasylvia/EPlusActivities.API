using System.Threading.Tasks;
using EPlusActivities.API.Infrastructure.ActionResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EPlusActivities.API.Infrastructure.Filters
{
    public class EPlusAuthorizationFilter : IAsyncAuthorizationFilter
    {
        public Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            switch (context.Result)
            {
                case UnauthorizedResult ur:
                    var apiResult = new ApiResult(ur);
                    context.HttpContext.Response.StatusCode = ur.StatusCode;
                    context.Result = apiResult;
                    break;
                case NotFoundResult nfr:
                    apiResult = new ApiResult(nfr);
                    context.HttpContext.Response.StatusCode = nfr.StatusCode;
                    context.Result = apiResult;
                    break;
                default:
                    break;
            }
            return Task.CompletedTask;
        }
    }
}
