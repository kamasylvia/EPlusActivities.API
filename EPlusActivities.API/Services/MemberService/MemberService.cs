using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using AutoMapper;
using Elf.WebAPI.Attributes;
using EPlusActivities.API.Dtos.MemberDtos;
using EPlusActivities.API.Entities;
using EPlusActivities.API.Infrastructure.Attributes;
using EPlusActivities.API.Infrastructure.Enums;
using EPlusActivities.API.Infrastructure.Exceptions;
using EPlusActivities.API.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EPlusActivities.API.Services.MemberService
{
    [AutomaticDependencyInjection(ServiceLifetime.Scoped)]
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
        /// <param name="channelCode">渠道号</param>
        /// <returns></returns>
        public async Task<MemberForGetDto> GetMemberAsync(string phone, ChannelCode channelCode)
        {
            var channel = channelCode.ToString().ToLower();

            var response = await _httpClientFactory
                .CreateClient()
                .PostAsJsonAsync(
                    new UriBuilder(
                        scheme: _configuration["MemberServiceUriBuilder:Scheme"],
                        host: _configuration["MemberServiceUriBuilder:Host"],
                        port: Convert.ToInt32(_configuration["MemberServiceUriBuilder:Port"]),
                        pathValue: string.Join(
                            '/',
                            _configuration["MemberServiceUriBuilder:PreChannelCodePathValue"],
                            channel,
                            _configuration[
                                "MemberServiceUriBuilder:PostChannelCodeGetMemberInfoRequest"
                            ]
                        )
                    ).Uri,
                    new { mobile = phone }
                );
            var result = await response.Content.ReadFromJsonAsync<MemberForGetDto>();

            if (result.Header.Code != "0000")
            {
                _logger.LogError("获取会员信息失败：", result.Header.Message);
                throw new RemoteServiceException($"获取会员信息失败：{result.Header.Message}");
            }

            return result;
        }

        public async Task<MemberForReleaseCouponResponseDto> ReleaseCouponAsync(
            ChannelCode channelCode,
            MemberForReleaseCouponRequestDto requestDto
        )
        {
            var channel = channelCode.ToString().ToLower();

            var response = await _httpClientFactory
                .CreateClient()
                .PostAsJsonAsync(
                    new UriBuilder(
                        scheme: _configuration["MemberServiceUriBuilder:Scheme"],
                        host: _configuration["MemberServiceUriBuilder:Host"],
                        port: Convert.ToInt32(_configuration["MemberServiceUriBuilder:Port"]),
                        pathValue: string.Join(
                            '/',
                            _configuration["MemberServiceUriBuilder:PreChannelCodePathValue"],
                            channel,
                            _configuration[
                                "MemberServiceUriBuilder:PostChannelReleaseCouponRequest"
                            ]
                        )
                    ).Uri,
                    requestDto
                );
            var responseDto =
                await response.Content.ReadFromJsonAsync<MemberForReleaseCouponResponseDto>();

            if (responseDto.Header.Code != "0000")
            {
                _logger.LogError("发放优惠券失败：", responseDto.Header.Message);
                throw new RemoteServiceException($"发放优惠券失败：{responseDto.Header.Message}");
            }

            return responseDto;
        }

        public async Task<MemberForUpdateCreditResponseDto> UpdateCreditAsync(
            Guid userId,
            ChannelCode channelCode,
            MemberForUpdateCreditRequestDto requestDto
        )
        {
            var channel = channelCode.ToString().ToLower();

            var response = await _httpClientFactory
                .CreateClient()
                .PostAsJsonAsync(
                    new UriBuilder(
                        scheme: _configuration["MemberServiceUriBuilder:Scheme"],
                        host: _configuration["MemberServiceUriBuilder:Host"],
                        port: Convert.ToInt32(_configuration["MemberServiceUriBuilder:Port"]),
                        pathValue: string.Join(
                            '/',
                            _configuration["MemberServiceUriBuilder:PreChannelCodePathValue"],
                            channel,
                            _configuration["MemberServiceUriBuilder:PostChannelUpdateCreditRequest"]
                        )
                    ).Uri,
                    requestDto
                );

            var responseDto =
                await response.Content.ReadFromJsonAsync<MemberForUpdateCreditResponseDto>();

            if (responseDto.Header.Code != "0000")
            {
                _logger.LogError("更新会员积分失败：", responseDto.Header.Message);
                throw new RemoteServiceException($"更新会员积分失败：{responseDto.Header.Message}");
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
                throw new DatabaseUpdateException();
            }
            #endregion

            return responseDto;
        }
    }
}
