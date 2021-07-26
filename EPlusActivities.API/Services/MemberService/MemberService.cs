using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.DTOs.MemberDtos;
using Microsoft.Extensions.Logging;

namespace EPlusActivities.API.Services.MemberService
{
    public class MemberService : IMemberService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        private readonly ILogger<MemberService> _logger;

        private readonly IMapper _mapper;

        public MemberService(
            IHttpClientFactory httpClientFactory,
            ILogger<MemberService> logger,
            IMapper mapper
        ) {
            _httpClientFactory =
                httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<MemberForGetDto> GetMemberAsync(string phone)
        {
            var channelCode = "test";
            var requestUri = $"http://10.10.34.218:9080/apis/member/eroc/{channelCode}/get/1.0.0";
            var httpClient = _httpClientFactory.CreateClient();
            var response = await httpClient.PostAsJsonAsync(requestUri, new { mobile = phone });
            var memberDto = await response.Content.ReadFromJsonAsync<MemberForGetDto>();

            if (memberDto.Header.Code != "0000")
            {
                _logger.LogError("获取会员信息失败：", memberDto.Header.Message);
            }

            return memberDto;
        }
    }
}
