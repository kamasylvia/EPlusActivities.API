using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EPlusActivities.API.Infrastructure.ActionResults
{
    public class InternalServerErrorObjectResult : ObjectResult
    {
        public InternalServerErrorObjectResult(object error) : base(error)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
            if (error is string err)
            {
                Value = new ProblemDetails
                {
                    Type = "https://httpstatuses.com/500",
                    Status = StatusCodes.Status500InternalServerError,
                    Title = "Internal Server Error",
                    Detail = err
                };
            }
        }
    }
}
