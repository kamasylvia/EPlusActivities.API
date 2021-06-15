using System;
using System.Collections.Generic;
using System.Linq;
using OtpNet;

namespace IdentityServer.Helpers
{
    public class TotpHelper
    {
        private static IDictionary<string, Totp> _totps = new Dictionary<string, Totp>();
        private TotpHelper()
        {
        }

        public static string GetCode(string phoneNumber)
        {
            System.Console.WriteLine("GetCode working");
            // 生成验证码
            var secretKey = KeyGeneration.GenerateRandomKey(20);

            /// <summary>
            /// Totp() 构造函数参数说明：
            /// </summary>
            /// <param name="step">验证码有效期。单位为秒，默认是 30 秒。推荐值为 300-600，过短的有效期会导致验证失败。</param>
            /// <param name="totpSize">验证码长度。默认为 6 位数。</param>
            /// <returns></returns>
            var totp = new Totp(secretKey: secretKey, mode: OtpHashMode.Sha512, step: 300);
            
            _totps[phoneNumber] = totp;
            return totp.ComputeTotp(DateTime.Now);
        }

        public static bool Validate(string phoneNumber, string code)
        {
            Totp totp;
            long timeWindowUsed;
            System.Console.WriteLine("Validate is working");
            if (_totps.TryGetValue(phoneNumber, out totp))
            {
                _totps.Remove(phoneNumber);
                return totp.VerifyTotp(DateTime.Now, code, out timeWindowUsed);
            }
            return false;
        }

        public static void CleanTotpDictionary()
        {
            System.Console.WriteLine("Cleaner is working");
            var expiredPairs = _totps.Where(kv => kv.Value.RemainingSeconds() <= 0);
            foreach (var item in expiredPairs)
            {
                _totps.Remove(item.Key);
            }
            System.Console.WriteLine("Cleaner is done");
        }
    }
}