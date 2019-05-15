

using System.Text;
using pluginmanager.Communication;

namespace pluginmanager
{
    class MainClass
    {
        public static void Main(string[] args)
        {

            //PluginLoader pluginLoader = new PluginLoader("./");

            Logger.Log("main", "started");
            PubSub pubSub = new PubSub("asdasdasdasdassdgdf");
            pubSub.Subscribe("test", (obj) => Logger.Log("main", Encoding.UTF8.GetString(obj, 0, obj.Length)));
            _ = pubSub.Publish("test", "yooo was geht ab");

            while (true) { }
        }
      

    }
}