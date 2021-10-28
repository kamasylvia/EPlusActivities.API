using System.Net.Http;
using System.Threading.Tasks;
using EPlusActivities.API.Application.Commands.SmsCommands;
using EPlusActivities.API.Dtos;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Services.IdentityServer
{
    public interface ISmsService
    {
        Task<HttpResponseMessage> SendAsync(string phoneNumber, string token);

        Task<ApplicationUser> GetSmsUserAsync(GetVerificationCodeCommand smsDto);
    }
}
