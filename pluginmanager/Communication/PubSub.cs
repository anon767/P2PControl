using System;
using System.Threading;
using System.Threading.Tasks;
using Ipfs.CoreApi;
using Ipfs.Engine;

namespace pluginmanager.Communication
{
    public class PubSub
    {
         private ICoreApi coreApi;
        public PubSub(string passphraset)
        {
            string passphrase = "this is not a secure pass phrase";

            var engine = new IpfsEngine(passphrase.ToCharArray());
            engine.StartAsync().Wait();
            coreApi = engine;
          

        }
        public async Task Publish(string topic, string message)
        {
            await coreApi.PubSub.PublishAsync(topic, message);

        }
        public void Subscribe(string topic, Action<byte[]> handler)
        {
            var cs = new CancellationTokenSource();
            coreApi.PubSub.SubscribeAsync(topic, msg =>
            {
                Logger.Log(ToString(), $"Received from {topic}");
                handler.Invoke(msg.DataBytes);
            }, cs.Token);

        }
    }
}
