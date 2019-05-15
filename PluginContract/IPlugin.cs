namespace PluginContract
{
    public interface IPlugin
    {
        string Name { get; }
        void Do();
    }
}