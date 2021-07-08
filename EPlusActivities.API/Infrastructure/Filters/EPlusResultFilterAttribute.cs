using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace EPlusActivities.API.Infrastructure.Filters
{
    public class EPlusResultFilterAttribute : ResultFilterAttribute, IAsyncAlwaysRunResultFilter
    {
        async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {
            await next();
        }
    }
}