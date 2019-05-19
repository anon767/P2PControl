using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.OpenSsl;

namespace pluginmanager.Communication
{
    public static class Verify
    {

        public static RSACryptoServiceProvider importkey(string key)
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 };
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);

            rsaProvider.ImportCspBlob(Convert.FromBase64String(key));

            return rsaProvider;
        }

        public static RSACryptoServiceProvider CreateKeyPair()
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 /* PROV_RSA_FULL */ };

            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(2048, cspParams);


            return rsaProvider;
        }
        public static Tuple<string,string> CreateKeyString()
            {
                CspParameters cspParams = new CspParameters { ProviderType = 1 /* PROV_RSA_FULL */ };

                RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(2048, cspParams);

                 string publicKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(false));
                 string privateKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(true));

                 return new Tuple<string, string>(privateKey, publicKey);
                }
        public static string SignData(string message, RSAParameters privateKey)
        {
            ASCIIEncoding byteConverter = new ASCIIEncoding();

            byte[] signedBytes;

            using (var rsa = new RSACryptoServiceProvider())
            {
                // Write the message to a byte array using ASCII as the encoding.
                byte[] originalData = byteConverter.GetBytes(message);

                try
                {
                    // Import the private key used for signing the message
                    rsa.ImportParameters(privateKey);

                    // Sign the data, using SHA512 as the hashing algorithm 
                    signedBytes = rsa.SignData(originalData, CryptoConfig.MapNameToOID(config.SigningAlgo));
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                    return null;
                }
                finally
                {
                    // Set the keycontainer to be cleared when rsa is garbage collected.
                    rsa.PersistKeyInCsp = false;
                }
            }
            // Convert the byte array back to a string message
            return Convert.ToBase64String(signedBytes);
        }

        public static bool VerifyData(string originalMessage, string signedMessage, RSAParameters publicKey)
        {
            bool success = false;
            using (var rsa = new RSACryptoServiceProvider())
            {
                ASCIIEncoding byteConverter = new ASCIIEncoding();

                byte[] bytesToVerify = byteConverter.GetBytes(originalMessage);
                byte[] signedBytes = Convert.FromBase64String(signedMessage);

                try
                {
                    rsa.ImportParameters(publicKey);

                    success = rsa.VerifyData(bytesToVerify, CryptoConfig.MapNameToOID(config.SigningAlgo), signedBytes);
                }
                catch (CryptographicException e)
                {
                    Console.WriteLine(e.Message);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
            return success;
        }
    }
}
