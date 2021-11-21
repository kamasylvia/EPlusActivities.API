using System.ComponentModel.DataAnnotations;
using MediatR;

namespace EPlusActivities.API.Application.Queries.SmsQueries
{
    public class GetVerificationCodeQuery : IRequest
    {
        [Required]
        [Phone]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "手机号必须为 11 位数字。")]
        public string PhoneNumber { get; set; }
    }
}
