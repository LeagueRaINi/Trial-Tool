using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace AdobeReset.Helpers
{
    internal class RandomGenerator
    {
        public static string String(int length, IReadOnlyList<char> chars)
        {
            var data = new byte[1];
            using (var crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetNonZeroBytes(data);
                data = new byte[length];
                crypto.GetNonZeroBytes(data);
            }

            var result = new StringBuilder(length);
            foreach (var b in data)
                result.Append(chars[b % (chars.Count)]);

            return result.ToString();
        }
    }
}
