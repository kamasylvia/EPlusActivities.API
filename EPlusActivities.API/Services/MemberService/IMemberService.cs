using System;
using System.Threading.Tasks;
using EPlusActivities.API.Dtos.MemberDtos;
using EPlusActivities.API.Infrastructure.Enums;

namespace EPlusActivities.API.Services.MemberService
{
    public interface IMemberService
    {
        Task<MemberForGetDto> GetMemberAsync(string phone, ChannelCode channelCode);
        Task<MemberForUpdateCreditResponseDto> UpdateCreditAsync(
            Guid userId,
            ChannelCode channelCode,
            MemberForUpdateCreditRequestDto requestDto
        );

        Task<MemberForReleaseCouponResponseDto> ReleaseCouponAsync(
            ChannelCode channelCode,
            MemberForReleaseCouponRequestDto requestDto
        );
    }
}
