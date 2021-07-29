using System.Threading.Tasks;
using EPlusActivities.API.DTOs.MemberDtos;
using Newtonsoft.Json.Linq;

namespace EPlusActivities.API.Services.MemberService
{
    public interface IMemberService
    {
        Task<(bool, MemberForGetDto)> GetMemberAsync(string phone);
        Task<(bool, MemberForUpdateCreditResponseDto)> UpdateCreditAsync(MemberForUpdateCreditRequestDto requestDto);
    }
}
