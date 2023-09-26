using System.Security.Cryptography;
using System.Text;

namespace Seunghak.Common
{
    class CommonUtil
    {
        public static long PoolRemoveSecTime = 600;
        public static string StringToMD5(string str)
        {
            StringBuilder MD5Str = new StringBuilder();
            byte[] byteArr = Encoding.ASCII.GetBytes(str);
            byte[] resultArr = (new MD5CryptoServiceProvider()).ComputeHash(byteArr);

            for (int cnti = 0; cnti < resultArr.Length; cnti++)
            {
                MD5Str.Append(resultArr[cnti].ToString("X2"));
            }
            return MD5Str.ToString();
        }
        public static string MD5ToString(string md5str)
        {
            byte[] bytePassword = Encoding.UTF8.GetBytes(md5str);
            string emptymd5;

            using (MD5 md5 = MD5.Create())
            {
                byte[] byteHashedPassword = md5.ComputeHash(bytePassword);
                emptymd5 = byteHashedPassword.ToString();
            }
            return emptymd5;
        }
    }
}
