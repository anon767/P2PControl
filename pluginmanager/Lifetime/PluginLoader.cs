using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using PluginContract;

namespace pluginmanager
{
    public class PluginLoader
    {
        private const string PluginEnding = "*.dll";

        ICollection<IPlugin> plugins;
        public PluginLoader(String path)
        {
            GetInstances(FilterPlugins(LoadAssemblies(GetDLLs(path))));
        }

        private string[] GetDLLs(String path)
        {
            string[] dllFileNames = null;
            if (Directory.Exists(path))
            {
                dllFileNames = Directory.GetFiles(path, PluginEnding);
            }
            return dllFileNames;
        }

        private ICollection<Assembly> LoadAssemblies(string[] dllFileNames)
        {
            ICollection<Assembly> assemblies = new List<Assembly>(dllFileNames.Length);
            dllFileNames.ToList().FindAll((dllFile) => !dllFile.Equals(null)).ForEach((dllFile) =>
            {
                AssemblyName an = AssemblyName.GetAssemblyName(dllFile);
                Assembly assembly = Assembly.Load(an);
                assemblies.Add(assembly);
            });
            return assemblies;
        }

        private ICollection<Type> FilterPlugins(ICollection<Assembly> assemblies)
        {
            Type pluginType = typeof(IPlugin);
            ICollection<Type> pluginTypes = new List<Type>();
            assemblies.ToList().FindAll((assembly) => assembly != null).ForEach((assembly) =>
              {
                      Type[] types = assembly.GetTypes();
                      types.ToList()
                          .FindAll((type) => (type.IsInterface || type.IsAbstract) && type.GetInterface(pluginType.FullName) != null)
                          .ForEach((type) => pluginTypes.Add(type));
              });
            return pluginTypes;
        }

        public void GetInstances(ICollection<Type> pluginTypes)
        {
            plugins = new List<IPlugin>(pluginTypes.Count);
            pluginTypes.ToList().ForEach((type) =>
            {
                IPlugin plugin = (IPlugin)Activator.CreateInstance(type);
                plugins.Add(plugin);
            });
        }
    }
}
