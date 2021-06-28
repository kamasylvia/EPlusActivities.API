using System.Net.Http;
using System.Threading.Tasks;
using EPlusActivities.DTOs;
using EPlusActivities.Entities;

namespace EPlusActivities.Services
{
    public interface ISmsService
    {
        Task<HttpResponseMessage> SendAsync(string phoneNumber, string token);
        Task<ApplicationUser> GetSmsUserAsync(SmsDto smsDto);
    }
}