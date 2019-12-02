using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Autofac.Util;
using Plugin;
using PluginHost;

namespace Host
{
    public class PluginFinder<TPlugin> where TPlugin : IPlugin
    {
        public Dictionary<string,HostAssemblyLoadContext> HostsDictionary = new Dictionary<string, HostAssemblyLoadContext>();
        public PluginFinder() { }

        public IReadOnlyCollection<string> FindAssemliesWithPlugins(string path)
        {
            string[] assemblies = Directory.GetFiles(path, "*.dll", new EnumerationOptions() { RecurseSubdirectories = true });
            return assemblies; 
        }

        private IReadOnlyCollection<Assembly> FindPluginsInAssemblies(string[] assemblyPaths)
        {
            var assemblyPluginInfos = new List<Assembly>();
            foreach (var assemblyPath in assemblyPaths)
            {
                var pluginFinderAssemblyContext = new HostAssemblyLoadContext(assemblyPath);
                var assembly = pluginFinderAssemblyContext.LoadFromAssemblyPath(assemblyPath);
              
                if (GetPluginTypes(assembly).Any())
                {
                    HostsDictionary.Add(assembly.FullName,pluginFinderAssemblyContext);
                    assemblyPluginInfos.Add(assembly);
                }
            }
            return assemblyPluginInfos;
        }

        public static IReadOnlyCollection<Type> GetPluginTypes(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(type =>
                    !type.IsAbstract &&
                    typeof(IPlugin).IsAssignableFrom(type))
                .ToArray();
        }

        public void Unload(string assemblyName)
        {
            HostsDictionary[assemblyName].Unload();
        }

    }

   

    public static class Extension
    {
        public static IEnumerable<Type> GetLoadableTypes(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException(nameof(assembly));
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException e)
            {
                return e.Types.Where(t => t != null);
            }
        }
    }
}
