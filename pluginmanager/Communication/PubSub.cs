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

            var engine = new IpfsEngine(config.PubSubPassPhrase.ToCharArray());
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
