using System;
using System.IO;
using System.Security.Cryptography;

namespace pluginmanager.Communication
{
    public class AESEncryption
    {
        public AESEncryption()
        {
        }
        private static readonly int iterations = 1000;

        public static string Encrypt(string input, string password)
        {
            byte[] encrypted;
            byte[] IV;
            byte[] Salt = GetSalt();
            byte[] Key = CreateKey(password, Salt);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.Padding = PaddingMode.PKCS7;
                aesAlg.Mode = CipherMode.CBC;

                aesAlg.GenerateIV();
                IV = aesAlg.IV;

                var encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(input);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            byte[] combinedIvSaltCt = new byte[Salt.Length + IV.Length + encrypted.Length];
            Array.Copy(Salt, 0, combinedIvSaltCt, 0, Salt.Length);
            Array.Copy(IV, 0, combinedIvSaltCt, Salt.Length, IV.Length);
            Array.Copy(encrypted, 0, combinedIvSaltCt, Salt.Length + IV.Length, encrypted.Length);

            return Convert.ToBase64String(combinedIvSaltCt);
        }

        public static string Decrypt(string input, string password)
        {
            byte[] inputAsByteArray;
            string plaintext = null;
            try
            {
                inputAsByteArray = Convert.FromBase64String(input);

                byte[] Salt = new byte[32];
                byte[] IV = new byte[16];
                byte[] Encoded = new byte[inputAsByteArray.Length - Salt.Length - IV.Length];

                Array.Copy(inputAsByteArray, 0, Salt, 0, Salt.Length);
                Array.Copy(inputAsByteArray, Salt.Length, IV, 0, IV.Length);
                Array.Copy(inputAsByteArray, Salt.Length + IV.Length, Encoded, 0, Encoded.Length);

                byte[] Key = CreateKey(password, Salt);

                using (Aes aesAlg = Aes.Create())
                {
                    aesAlg.Key = Key;
                    aesAlg.IV = IV;
                    aesAlg.Mode = CipherMode.CBC;
                    aesAlg.Padding = PaddingMode.PKCS7;

                    ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                    using (var msDecrypt = new MemoryStream(Encoded))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                plaintext = srDecrypt.ReadToEnd();
                            }
                        }
                    }
                }

                return plaintext;
            }
            catch (Exception e)
            {
                Logger.Log("AESEncryption", $"{e.Message}");
                return null;
            }
        }

        public static byte[] CreateKey(string password, byte[] salt)
        {
            using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, iterations))
                return rfc2898DeriveBytes.GetBytes(32);
        }

        private static byte[] GetSalt()
        {
            var salt = new byte[32];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return salt;
        }
    }
}
