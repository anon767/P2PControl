using System;
namespace pluginmanager
{
    public class Logger
    {
        public Logger()
        {
        }

        public static void Log(String modulename, String msg)
        {
            Console.WriteLine($"{modulename}: {msg}");
        }
    }
}
