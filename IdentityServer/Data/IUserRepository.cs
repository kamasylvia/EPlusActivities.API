using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Entities;

namespace IdentityServer.Data
{
    public interface IUserRepository
    {
        void Update(ApplicationUser user);
        Task<IEnumerable<ApplicationUser>> GetUsersAsync();
        Task<ApplicationUser> GetUserByIdAsync(Guid id);
        Task<ApplicationUser> GetUserByPhoneAsync(string phoneNumber);
        // Task<PagedList<MemberDto>> GetMembersAsync(UserParams userParams);
        // Task<MemberDto> GetMemberAsync(string username);

    }
}