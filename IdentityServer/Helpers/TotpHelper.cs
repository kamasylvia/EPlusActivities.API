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
            Totp totp;
            if (_totps.TryGetValue(phoneNumber, out totp))
            {
                if (totp.RemainingSeconds(DateTime.Now) <= 0)
                {
                    System.Console.WriteLine("Enter iff");
                    return totp.ComputeTotp(DateTime.Now);
                }
            }

            // 生成验证码
            var secretKey = KeyGeneration.GenerateRandomKey(20);
            // Totp() 的构造函数中加入参数 totpSize 可以设定验证码长度，默认是 6 位数。
            totp = new Totp(secretKey: secretKey, mode: OtpHashMode.Sha512);
            _totps[phoneNumber] = totp;
            return totp.ComputeTotp(DateTime.Now);
        }

        public static bool Validate(string phoneNumber, string code)
        {
            Totp totp;
            long timeWindowUsed;
            if (_totps.TryGetValue(phoneNumber, out totp))
            {
                _totps.Remove(phoneNumber);
                return totp.VerifyTotp(code, out timeWindowUsed);
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