namespace IdentityServer.Services.Authentication
{
    public class VerificationCodeService : IVerificationCodeService
    {
        public bool Validate(string phone, string code)
        {
            return true;
            throw new System.NotImplementedException();
        }
    }
}