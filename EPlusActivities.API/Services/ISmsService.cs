using System.Net.Http;
using System.Threading.Tasks;
using EPlusActivities.API.DTOs;
using EPlusActivities.API.Entities;

namespace EPlusActivities.API.Services
{
    public interface ISmsService
    {
        Task<HttpResponseMessage> SendAsync(string phoneNumber, string token);

        Task<ApplicationUser> GetSmsUserAsync(SmsDto smsDto);
    }
}
