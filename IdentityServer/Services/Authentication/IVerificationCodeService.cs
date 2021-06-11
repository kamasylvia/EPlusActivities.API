namespace IdentityServer.Services.Authentication
{
    public interface IVerificationCodeService
    {
        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="phone">手机号</param>
        /// <param name="code">验证码</param>
        /// <returns></returns>
        bool Validate(string phone, string code);
    }
}