using NUnit.Framework;
using pluginmanager;
using pluginmanager.Communication;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Tests
{
    [TestFixture()]
    public class EncryptDecryptTest
    {


        [Test()]
        public void TestEncryptDecrypt()
        {
            string encrypted = AESEncryption.Encrypt("message", "password");
            Assert.AreEqual("message", AESEncryption.Decrypt(encrypted, "password"), "Decrypted(Encrypted(message)) == mesage");
            Assert.AreNotEqual("message", encrypted, "Encrypted(message) != message");
        }
        [Test()]
        public void TestVerify()
        {

            RSACryptoServiceProvider rsa = Verify.CreateKeyPair();
           

            string message = "2017-04-10T09:37:35.351Z";
            string signedMessage = Verify.SignData(message, rsa.ExportParameters(true));
            bool success = Verify.VerifyData(message, signedMessage, rsa.ExportParameters(false));


            Assert.True(success, "verify(signed(message,privkey),pubkey) == true");
        }
        [Test()]
        public void TestVerify2()
        {
            Tuple<string, string> keypair = Verify.CreateKeyString();
            RSACryptoServiceProvider rsa = Verify.importkey(keypair.Item1);

            string message = "2017-04-10T09:37:35.351Z";
            string signedMessage = Verify.SignData(message, rsa.ExportParameters(true));
            bool success = Verify.VerifyData(message, signedMessage, rsa.ExportParameters(false));


            Assert.True(success, "verify(signed(message,privkey),pubkey) == true");
            Console.Out.WriteLine(keypair.Item2);
            Console.Out.WriteLine(keypair.Item1);

        }
    }
}
