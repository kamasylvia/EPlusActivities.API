using System.Threading.Tasks;
using EPlusActivities.API.DTOs.MemberDtos;
using Newtonsoft.Json.Linq;

namespace EPlusActivities.API.Services.MemberService
{
    public interface IMemberService
    {
        Task<MemberForGetDto> GetMemberAsync(string phone);
    }
}
