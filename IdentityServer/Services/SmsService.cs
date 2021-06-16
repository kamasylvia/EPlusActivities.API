using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace IdentityServer.Services
{
    public class SmsService : ISmsService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SmsService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }


        public async Task<HttpResponseMessage> SendAsync(string phoneNumber, string token)
        {
            var uriString = "http://10.10.34.202:20015/api/sms/message/send";
            var contentObject = new
            {
                messageContent = $"【农工商】验证码: {token}。您正在验证农工商用户，10 分钟内同一手机号最多发送 3 次验证码，每个验证码有效期为 8 分钟，感谢您的支持！",
                messageKey = "sms_test",
                messageTarget = phoneNumber
            };
            var jsonContent = JsonContent.Create(contentObject);
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsync(uriString, jsonContent);

            return response;
        }
    }
}