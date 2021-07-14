using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EPlusActivities.API.Entities;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;

namespace EPlusActivities.API.Infrastructure.Identity
{
    /// <summary>
    /// /// Often IdentityServer requires identity information about users when creating tokens or when

    /// handling requests to the userinfo or introspection endpoints. By default, IdentityServer only has the

    /// claims in the authentication cookie to draw upon for this identity data.
    ///

    /// It is impractical to put all of the possible claims needed for users into the cookie, so IdentityServer

    /// defines an extensibility point for allowing claims to be dynamically loaded as needed for a user. This

    /// extensibility point is the IProfileService and it is common for a developer to implement this

    /// interface to access a custom database or API that contains the identity data for users.
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        private readonly RoleManager<ApplicationRole> _roleManager;

        public ProfileService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        /// <summary>
        /// 获取用户Claims
        /// 用户请求userinfo endpoint时会触发该方法
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId =
                context
                    .Subject
                    .Claims
                    .FirstOrDefault(c => c.Type == JwtClaimTypes.Subject)
                    .Value;
            var user = await _userManager.FindByIdAsync(subjectId);
            context.IssuedClaims = await GetClaimsFromUserAsync(user);
        }

        /// <summary>
        /// 判断用户是否可用
        /// Identity Server会确定用户是否有效
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task IsActiveAsync(IsActiveContext context)
        {
            var subjectId =
                context
                    .Subject
                    .Claims
                    .FirstOrDefault(c => c.Type == JwtClaimTypes.Subject)
                    .Value;
            var user = await _userManager.FindByIdAsync(subjectId);
            context.IsActive = user != null; //该用户是否已经激活，可用，否则不能接受token
        }

        public async Task<List<Claim>>
        GetClaimsFromUserAsync(ApplicationUser user)
        {
            var claims =
                new List<Claim> {
                    new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
                    new Claim(JwtClaimTypes.PreferredUserName,
                        user.PhoneNumber),
                    new Claim(JwtClaimTypes.PhoneNumber, user.PhoneNumber),
                    new Claim("is_member", user.IsMember.ToString()),
                    new Claim("credit", user.Credit.ToString()),
                    new Claim("register_channel", user.RegisterChannel),
                    new Claim("login_channel", user.LoginChannel)
                };

            var roles = await _userManager.GetRolesAsync(user);

            // var claims = await _userManager.GetClaimsAsync(user);
            claims
                .AddRange(roles
                    .Select(role => new Claim(JwtClaimTypes.Role, role)));

            return claims.ToList();
        }
    }
}
