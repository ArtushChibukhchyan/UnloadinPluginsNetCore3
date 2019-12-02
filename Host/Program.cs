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
        // Resolver of the locations of the assemblies that are dependencies of the
        // main plugin assembly.
        private AssemblyDependencyResolver _resolver;
        public string Path { get; set; }
        public string PluginName { get; set; }
        public IPlugin Plugin { get;  set; }
        public string PluginMessage { get; set; }

        public HostAssemblyLoadContext(string pluginPath) : base(isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(pluginPath);
            Path = pluginPath;
        }

        // The Load method override causes all the dependencies present in the plugin's binary directory to get loaded
        // into the HostAssemblyLoadContext together with the plugin assembly itself.
        // NOTE: The Interface assembly must not be present in the plugin's binary directory, otherwise we would
        // end up with the assembly being loaded twice. Once in the default context and once in the HostAssemblyLoadContext.
        // The types present on the host and plugin side would then not match even though they would have the same names.
        protected override Assembly Load(AssemblyName name)
        {
            string assemblyPath = _resolver.ResolveAssemblyToPath(name);
            if (assemblyPath != null)
            {
                Console.WriteLine($"Loading assembly {assemblyPath} into the HostAssemblyLoadContext");
                return LoadFromAssemblyPath(assemblyPath);
            }

            return null;
        }
    }

    public class Program
    {
        public static Dictionary<HostAssemblyLoadContext,WeakReference> DictionaryHostWeakReferences = new Dictionary<HostAssemblyLoadContext, WeakReference>();
        static void Main(string[] args)
        {
            Program a = new Program();
            a.Test();

            Console.ReadKey();

        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Unload(WeakReference testAlcWeakRef, HostAssemblyLoadContext alc, string pluginFullPath)
        {
            alc.Unload();
            alc = null;

            for (int i = 0; testAlcWeakRef.IsAlive && (i < 10); i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            Console.WriteLine($"is alive: {testAlcWeakRef.IsAlive}");

            try
            {

                File.Delete(pluginFullPath);
                Console.WriteLine("Deleted");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(out WeakReference testAlcWeakRef, out HostAssemblyLoadContext alc, string path)
        {
            string name = string.Empty;
            alc = new HostAssemblyLoadContext(path);
            testAlcWeakRef = new WeakReference(alc);

            Assembly a = alc.LoadFromAssemblyPath(path);

            var validPluginTypes = PluginFinder<IPlugin>.GetPluginTypes(a);
            foreach (var pluginType in validPluginTypes)
            {
                var plutinInstance = (IPlugin)Activator.CreateInstance(pluginType);
                string result = plutinInstance.GetMessage();
               // Console.WriteLine($"Response from the plugin: GetVersion(): {version}, GetMessage(): {result}");
                alc.PluginName = plutinInstance.Name;
                //alc.Plugin = plutinInstance;
                DictionaryHostWeakReferences.Add(alc, testAlcWeakRef);
            }
        }
        public static IReadOnlyCollection<Type> GetPluginTypes(Assembly assembly)
        {
            return assembly.GetTypes()
                .Where(type =>
                    !type.IsAbstract &&
                    typeof(IPlugin).IsAssignableFrom(type))
                .ToArray();
        }
        public void Test()
        {
            WeakReference hostAlcWeakRef;
            HostAssemblyLoadContext alc;

            var pluginFinder = new PluginFinder<IPlugin>();
            var assemblyPaths = pluginFinder.FindAssemliesWithPlugins(@"D:\c#\Plugins").ToArray();
            foreach (string assemblyPath in assemblyPaths)
            {
                Execute(out hostAlcWeakRef, out alc, assemblyPath);
            }
            for (int i = 0; i<10; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }


            var plugin = DictionaryHostWeakReferences.FirstOrDefault(x => x.Key.PluginName == "Plugin 2");
            if (plugin.Key != null)
            {
                Unload(plugin.Value, plugin.Key, plugin.Key.Path);
            }
            Console.ReadKey();
      
        }
    }
}
