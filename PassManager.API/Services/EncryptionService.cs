using System.Security.Cryptography;
using System.Text;

namespace PassManager.API.Services
{
    public class EncryptionService
    {
        private readonly IConfiguration _configuration;
        private readonly byte[] _key;

        public EncryptionService(IConfiguration configuration)
        {
            _configuration = configuration;
            string keyString = _configuration["JwtSettings:Secret"];
            _key = Encoding.UTF8.GetBytes(keyString.Substring(0, 32));
        }

        public string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.GenerateIV();
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        byte[] iv = aesAlg.IV;
                        byte[] encryptedData = msEncrypt.ToArray();
                        byte[] combined = new byte[iv.Length + encryptedData.Length];
                        Array.Copy(iv, 0, combined, 0, iv.Length);
                        Array.Copy(encryptedData, 0, combined, iv.Length, encryptedData.Length);

                        return Convert.ToBase64String(combined);
                    }
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return null;

            byte[] combined = Convert.FromBase64String(cipherText);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.Mode = CipherMode.CBC;
                aesAlg.Padding = PaddingMode.PKCS7;

                byte[] iv = new byte[aesAlg.BlockSize / 8];
                byte[] encryptedData = new byte[combined.Length - iv.Length];
                Array.Copy(combined, 0, iv, 0, iv.Length);
                Array.Copy(combined, iv.Length, encryptedData, 0, encryptedData.Length);
                aesAlg.IV = iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(encryptedData))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}