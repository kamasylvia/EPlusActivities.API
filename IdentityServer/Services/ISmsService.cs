using System.Net.Http;
using System.Threading.Tasks;
using IdentityServer.DTOs;
using IdentityServer.Entities;

namespace IdentityServer.Services
{
    public interface ISmsService
    {
        Task<HttpResponseMessage> SendAsync(string phoneNumber, string token);
        Task<ApplicationUser> GetSmsUserAsync(SmsDto smsDto);
    }
}