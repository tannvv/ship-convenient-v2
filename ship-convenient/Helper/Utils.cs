using System.Security.Cryptography;
using System.Text;

namespace ship_convenient.Helper
{
    public class Utils
    {
        public static double GetPercent(string text)
        {
            double percent = double.Parse(text) / 100;
            if (percent < 0 || percent > 1) throw new ArgumentException("Percent have from 0 to 1");
            return percent;
        }

        public static String HmacSHA512(string key, String inputData)
        {
            var hash = new StringBuilder();
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] inputBytes = Encoding.UTF8.GetBytes(inputData);
            using (var hmac = new HMACSHA512(keyBytes))
            {
                byte[] hashValue = hmac.ComputeHash(inputBytes);
                foreach (var theByte in hashValue)
                {
                    hash.Append(theByte.ToString("x2"));
                }
            }

            return hash.ToString();
        }
    }
}
