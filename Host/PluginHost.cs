using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using Plugin;
using PluginHost;

namespace Host
{
    public class PluginHost<TPlugin> where TPlugin : IPlugin
    {
        private Dictionary<string, TPlugin> _plugins = new Dictionary<string, TPlugin>();
        public List<HostAssemblyLoadContext> hosts = new List<HostAssemblyLoadContext>();

        public PluginHost()
        {
            //_pluginAssemblyLoadingContext = new PluginLoadContext("PluginAssemblyContext");
        }

        public TPlugin GetPlugin(string pluginName)
        {
            return _plugins[pluginName];
        }

        public IReadOnlyCollection<TPlugin> GetPlugins()
        {
            return _plugins.Values;
        }

        public void LoadPlugins(IReadOnlyCollection<string> assembliesWithPlugins)
        {
            foreach (var assemblyPath in assembliesWithPlugins)
            {
                HostAssemblyLoadContext _pluginAssemblyLoadingContext = new HostAssemblyLoadContext(assemblyPath);
               var assembly = _pluginAssemblyLoadingContext.LoadFromAssemblyPath(assemblyPath);
                var validPluginTypes = PluginFinder<TPlugin>.GetPluginTypes(assembly);
                foreach (var pluginType in validPluginTypes)
                {
                    var plutinInstance = (TPlugin)Activator.CreateInstance(pluginType);
                    hosts.Add(_pluginAssemblyLoadingContext);
                    RegisterPlugin(plutinInstance);
                }

            }
        }

        private void RegisterPlugin(TPlugin plutinInstance)
        {
            _plugins.Add(plutinInstance.Name,plutinInstance);
        }

        public void Unload()
        {
            _plugins.Clear();
          //  _pluginAssemblyLoadingContext.Unload();
            foreach (HostAssemblyLoadContext host in hosts)
            {
                host.Unload();
            }
        }
    }

  
}
