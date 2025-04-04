using System.Collections;
using System.Security.Cryptography;

namespace ServidorApiRestaurante.Controllers
{
    public class AESCipher
    {
        public static string AESKeyBase64 = "r0+uZQWn+tOVpF+91XMoImS6Bg+nSnusjKVqL3XaYsE=";
        public static string AESIVBase64 = "jta0tH/Hine/qclXfvF6QQ==";

        public static byte[] key = Convert.FromBase64String(AESKeyBase64);
        public static byte[] iv = Convert.FromBase64String(AESIVBase64);


        public static string Encrypt(string simpleText)
        {
            byte[] cipheredtextInBytes;
            using (Aes aes = Aes.Create())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(key, iv);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter(cryptoStream))
                        {
                            streamWriter.Write(simpleText);
                        }

                        cipheredtextInBytes = memoryStream.ToArray();
                    }
                }
            }
            return Convert.ToBase64String(cipheredtextInBytes); 
        }

        public static string Decrypt(string cipherTextBase64)
        {
            using Aes aesAlg = Aes.Create();
            aesAlg.Key = key;
            aesAlg.IV = iv;
            aesAlg.Mode = CipherMode.CBC;
            aesAlg.Padding = PaddingMode.PKCS7;

            ICryptoTransform decryptor = aesAlg.CreateDecryptor();
            byte[] cipherBytes = Convert.FromBase64String(cipherTextBase64);

            using MemoryStream msDecrypt = new(cipherBytes);
            using CryptoStream csDecrypt = new(msDecrypt, decryptor, CryptoStreamMode.Read);
            using StreamReader srDecrypt = new(csDecrypt);

            return srDecrypt.ReadToEnd();
        }
    }

}
