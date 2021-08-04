using System;
using System.Threading.Tasks;
using EPlusActivities.API.DTOs.MemberDtos;

namespace EPlusActivities.API.Services.MemberService
{
    public interface IMemberService
    {
        Task<(bool, MemberForGetDto)> GetMemberAsync(string phone);
        Task<(bool, MemberForUpdateCreditResponseDto)> UpdateCreditAsync(
            Guid userId,
            MemberForUpdateCreditRequestDto requestDto
        );

        Task<(bool, MemberForReleaseCouponResponseDto)> ReleaseCoouponAsync(
            MemberForReleaseCouponRequestDto requestDto
        );
    }
}
