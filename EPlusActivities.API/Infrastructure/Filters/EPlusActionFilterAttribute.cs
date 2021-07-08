using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EPlusActivities.API.Infrastructure.Filters
{
    public class EPlusActionFilterAttribute : ActionFilterAttribute
    {
        public async virtual Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            await next();
        }

    }
}