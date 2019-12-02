using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using Host;
using Plugin;

namespace PluginHost
{
    // This is a collectible (unloadable) AssemblyLoadContext that loads the dependencies
    // of the plugin from the plugin's binary directory.
    public class HostAssemblyLoadContext : AssemblyLoadContext
    {
 
        public string Path { get; set; }
        public IPlugin Plugin { get;  set; }

        public HostAssemblyLoadContext(string pluginPath) : base(isCollectible: true)
        {
            Path = pluginPath;
        }
      
    }
 
}
