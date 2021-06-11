using System.Threading.Tasks;

namespace IdentityServer.Services.Authentication
{
    public interface IUserService
    {
        /// <summary>
        /// 检查手机是否注册，如果没有就创建
        /// </summary>
        /// <param name="phone"></param>
        Task<int> CheckOrCreate(string phone);
    }
}