using pluginmanager.Communication;
using pluginmanager.Lifetime;

namespace pluginmanager
{
    class MainClass
    {
        public static void Main(string[] args)
        {

            PluginLoader pluginLoader = new PluginLoader("./");
            Register register = new Register();

            Logger.Log("main", "started");
            Dispatcher dispatcher = new Dispatcher();


            while (true) { }
        }
      

    }
}