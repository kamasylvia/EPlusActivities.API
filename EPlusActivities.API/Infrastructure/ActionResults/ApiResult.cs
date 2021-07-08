using Microsoft.AspNetCore.Mvc;

namespace EPlusActivities.API.Infrastructure.ActionResults
{
    public class ApiResult
    {
        public int? StatusCode { get; set; }
        public object Data { get; set; }
        public object Errors { get; set; }
        public bool Succeeded { get; set; }
        public ApiResult(ObjectResult objectResult)
        {
            StatusCode = objectResult.StatusCode;

            switch (objectResult)
            {
                case OkObjectResult:
                    Data = objectResult.Value;
                    Succeeded = true;
                    break;
                case AcceptedResult:
                case AcceptedAtActionResult:
                case AcceptedAtRouteResult:
                case CreatedResult:
                case CreatedAtActionResult:
                case CreatedAtRouteResult:
                    Succeeded = true;
                    break;
                default:
                    Errors = objectResult.Value;
                    Succeeded = false;
                    break;
            }
        }
    }
}