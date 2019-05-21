using System.Security.Cryptography;
using System.Text;

namespace AdobeReset.Helpers
{
    public static class Utils
    {
        public static string RandoGen(int length, string chars = "0123456789")
        {
            var buf = new byte[1];
            using (var crypto = new RNGCryptoServiceProvider()) {
                crypto.GetNonZeroBytes(buf);
                buf = new byte[length];
                crypto.GetNonZeroBytes(buf);
            }

            var result = new StringBuilder(length);
            foreach (var b in buf) {
                result.Append(chars[b % chars.Length]);
            }

            return result.ToString();
        }
    }
}
