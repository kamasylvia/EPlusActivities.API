using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace FileService.Infrastructure.ActionResults
{
    public class ApiResult : ActionResult
    {
        public int? StatusCode { get; set; }

        public object Data { get; set; }

        public object Errors { get; set; }

        public bool Succeeded { get; set; }

        public ApiResult() { }

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

        public ApiResult(StatusCodeResult statusCodeResult)
        {
            StatusCode = statusCodeResult.StatusCode;
            switch (StatusCode)
            {
                case 200:
                case 203: // 暂时无效
                    Data = "操作成功";
                    Succeeded = true;
                    break;
                default:
                    break;
            }
        }
    }
}
