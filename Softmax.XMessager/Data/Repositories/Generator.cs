using Softmax.XMessager.Data.Contracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Softmax.XMessager.Extensions;
using Softmax.XMessager.Data.Enums;

namespace Softmax.XMessager.Data.Repositories
{
    public class Generator : IGenerator
    {
        private readonly string PasswordHash = "P@@Sw0rd";
        private readonly string SaltKey = "S@LT&KEY";
        private readonly string VIKey = "@1B2c3D4e5F6g7H8";

        public Response<string> GenerateGuid()
        {
            return new Response<string>() { ResultType = ResultType.Success, Result = Guid.NewGuid().ToString() };
        }

        public Response<string> RandomNumber(int min, int max)
        {
            var random = new Random();
            return new Response<string>()
            {
                ResultType = ResultType.Success,
                Result = random.Next(min, max).ToString()
            };
        }

        public Response<string> DateCodeString()
        {
                var date = DateTime.Now;
                var year = date.Year;
                var month = date.Month;
                var day = date.Day;

                var mDay = string.Empty;
                var mMonth = string.Empty;

                mMonth = month < 10 ? "0" + month : month.ToString();
                mDay = day < 10 ? "0" + day : day.ToString();


                var result = year + mMonth + mDay;
                return new Response<string>()
                {
                    ResultType = ResultType.Success,
                    Result = result
                };


        }

        public Response<string> TempPassword(int num)
        {
            var password = GenerateGuid()
                .Result.Substring(0, num);
            var result = "Pass" + password + "@1";
            return new Response<string>()
            {
                ResultType = ResultType.Success,
                Result = result
            };
        }

        public Response<string> Encrypt(string plainText)
        {
            byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.Zeros };
            var encryptor = symmetricKey.CreateEncryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));

            byte[] cipherTextBytes;

            using (var memoryStream = new MemoryStream())
            {
                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                    cryptoStream.FlushFinalBlock();
                    cipherTextBytes = memoryStream.ToArray();
                    cryptoStream.Close();
                }
                memoryStream.Close();
            }
            var result= Convert.ToBase64String(cipherTextBytes);
            return new Response<string>()
            {
                ResultType = ResultType.Success,
                Result = result
            };
        }

        public Response<string> Decrypt(string encryptedText)
        {
            byte[] cipherTextBytes = Convert.FromBase64String(encryptedText);
            byte[] keyBytes = new Rfc2898DeriveBytes(PasswordHash, Encoding.ASCII.GetBytes(SaltKey)).GetBytes(256 / 8);
            var symmetricKey = new RijndaelManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.None };

            var decryptor = symmetricKey.CreateDecryptor(keyBytes, Encoding.ASCII.GetBytes(VIKey));
            var memoryStream = new MemoryStream(cipherTextBytes);
            var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
            byte[] plainTextBytes = new byte[cipherTextBytes.Length];

            int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
            memoryStream.Close();
            cryptoStream.Close();
            var result  = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount).TrimEnd("\0".ToCharArray());
            return new Response<string>()
            {
                ResultType = ResultType.Success,
                Result = result
            };
        }
    }
}
