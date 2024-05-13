using System.Security.Cryptography;
using System.Text;

namespace MES.UI.Commons
{
    public class Commons
    {
        public static string MKRandom(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            // 랜덤 숫자 생성기 인스턴스 생성
            Random random = new Random();

            // 랜덤한 10자리 문자열 생성
            char[] stringChars = new char[length];
            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }
            string randomString = new string(stringChars);

            //Console.WriteLine("랜덤한 10자리 문자열: " + randomString);

            return randomString;
        }

        public static string MKSHA256()
        {
            string input = MKRandom(20);
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                // 해시 값을 문자열로 변환하여 출력
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("x2")); // 각 바이트를 16진수로 변환하여 추가
                }
                string hashString = sb.ToString();
                //Console.WriteLine("SHA-256 해시 값: " + hashString);
                return hashString;
            }
        }
    }
}
