using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using EPlusActivities.API.Dtos.MemberDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace EPlusActivities.API.Services.MemberService
{
    public class MemberService : IMemberService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<MemberService> _logger;
        private readonly IRepository<Credit> _creditRepository;
        private readonly IMapper _mapper;

        public MemberService(
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory,
            ILogger<MemberService> logger,
            IRepository<Credit> creditRepository,
            IMapper mapper
        )
        {
            _configuration =
                configuration ?? throw new ArgumentNullException(nameof(configuration));
            _httpClientFactory =
                httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _creditRepository =
                creditRepository ?? throw new ArgumentNullException(nameof(creditRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// 获取会员信息
        /// </summary>
        /// <param name="phone">会员手机号</param>
        /// <returns></returns>
        public async Task<(bool, MemberForGetDto)> GetMemberAsync(string phone)
        {
            var response = await _httpClientFactory
                .CreateClient()
                .PostAsJsonAsync(
                    _configuration["MemberServiceUriBuilder:GetMemberInfoRequestUrl"],
                    new { mobile = phone }
                );
            var result = await response.Content.ReadFromJsonAsync<MemberForGetDto>();

            if (result.Header.Code != "0000")
            {
                _logger.LogError("获取会员信息失败：", result.Header.Message);
                return (false, result);
            }

            return (true, result);
        }

        public async Task<(bool, MemberForReleaseCouponResponseDto)> ReleaseCouponAsync(
            MemberForReleaseCouponRequestDto requestDto
        )
        {
            var response = await _httpClientFactory
                .CreateClient()
                .PostAsJsonAsync(
                    _configuration["MemberServiceUriBuilder:CouponRequestUrl"],
                    requestDto
                );
            var responseDto =
                await response.Content.ReadFromJsonAsync<MemberForReleaseCouponResponseDto>();

            if (responseDto.Header.Code != "0000")
            {
                _logger.LogError("发放优惠券失败：", responseDto.Header.Message);
                return (false, responseDto);
            }

            return (true, responseDto);
        }

        public async Task<(bool, MemberForUpdateCreditResponseDto)> UpdateCreditAsync(
            Guid userId,
            MemberForUpdateCreditRequestDto requestDto
        )
        {
            var response = await _httpClientFactory
                .CreateClient()
                .PostAsJsonAsync(
                    _configuration["MemberServiceUriBuilder:UpdateCreditRequestUrl"],
                    requestDto
                );

            var responseDto =
                await response.Content.ReadFromJsonAsync<MemberForUpdateCreditResponseDto>();

            if (responseDto.Header.Code != "0000")
            {
                _logger.LogError("更新会员积分失败：", responseDto.Header.Message);
                return (false, responseDto);
            }

            #region Database operations
            var credit = _mapper.Map<Credit>(requestDto);
            credit = _mapper.Map<MemberForUpdateCreditResponseDto, Credit>(responseDto, credit);
            credit.UserId = userId;
            await _creditRepository.AddAsync(credit);
            var result = await _creditRepository.SaveAsync();
            if (!result)
            {
                _logger.LogError("Update database exception.");
            }
            #endregion

            return (result, responseDto);
        }
    }
}
