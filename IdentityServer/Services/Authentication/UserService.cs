using System.Threading.Tasks;

namespace IdentityServer.Services.Authentication
{
    public class UserService : IUserService
    {
        public Task<int> CheckOrCreate(string phone)
        {
            throw new System.NotImplementedException();
        }
    }
}