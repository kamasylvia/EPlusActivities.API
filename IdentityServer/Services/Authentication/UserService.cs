using System.Threading.Tasks;

namespace IdentityServer.Services.Authentication
{
    public class UserService : IUserService
    {
        public async Task<int> CheckOrCreate(string phone)
        {
            return 1;
            throw new System.NotImplementedException();
        }
    }
}