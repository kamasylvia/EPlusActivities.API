using System.Net.Http;
using System.Threading.Tasks;

namespace IdentityServer.Services
{
    public interface ISmsService
    {
        Task<HttpResponseMessage> SendAsync(string phoneNumber, string token);
    }
}