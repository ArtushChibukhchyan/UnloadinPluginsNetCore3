using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Host;
using Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Plugin;
using PluginHost;

namespace WebMEF.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public static Dictionary<HostAssemblyLoadContext, WeakReference> DictionaryHostWeakReferences = new Dictionary<HostAssemblyLoadContext, WeakReference>();
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Unload(WeakReference testAlcWeakRef, HostAssemblyLoadContext alc, string pluginFullPath)
        {
            try
            {
                alc.Unload();
                alc = null;
                DictionaryHostWeakReferences.Remove(DictionaryHostWeakReferences
                    .FirstOrDefault(x => x.Key.Path == pluginFullPath).Key);
                for (int i = 0; testAlcWeakRef.IsAlive && (i < 10); i++)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                }

                try
                {
                    System.IO.File.Delete(pluginFullPath);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    _logger.LogError(e.Message);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }


        [MethodImpl(MethodImplOptions.NoInlining)]
        public void Execute(out WeakReference testAlcWeakRef, out HostAssemblyLoadContext alc, string path)
        {
            alc = new HostAssemblyLoadContext(path);
            testAlcWeakRef = new WeakReference(alc);

            Assembly a = alc.LoadFromAssemblyPath(path);

            var validPluginTypes = PluginFinder<IPlugin>.GetPluginTypes(a);
            foreach (var pluginType in validPluginTypes)
            {
                try
                {
                    IPlugin plutinInstance = (IPlugin)Activator.CreateInstance(pluginType);
                    alc.Plugin = new BasePlugin(plutinInstance);
                    if (DictionaryHostWeakReferences.Any(t => t.Key.Plugin.Id == plutinInstance.Id))
                        return;
                    DictionaryHostWeakReferences.Add(alc, testAlcWeakRef);
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                }

            }
        }
        public IActionResult Index()
        {

            List<IPlugin> plugins = new List<IPlugin>();
            WeakReference hostAlcWeakRef;
            HostAssemblyLoadContext alc;

            var pluginFinder = new PluginFinder<IPlugin>();
            var assemblyPaths = pluginFinder.FindAssemliesWithPlugins(@"D:\c#\Plugins").ToArray();
            foreach (string assemblyPath in assemblyPaths)
            {
                Execute(out hostAlcWeakRef, out alc, assemblyPath);
            }
            for (int i = 0; i < 10; i++)
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

            foreach (var dictionaryHostWeakReference in DictionaryHostWeakReferences)
            {
                plugins.Add(dictionaryHostWeakReference.Key.Plugin);
            }

            return View(plugins);
        }

        [HttpGet]
        public IActionResult DeletePlugin(string id)
        {
            var plugin = DictionaryHostWeakReferences.FirstOrDefault(x => x.Key.Plugin.Id.ToString() ==  id);
            if (plugin.Key != null)
            {
                Unload(plugin.Value, plugin.Key, plugin.Key.Path);
            }
            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult Upload(IFormFile file)
        {
            var plugin = DictionaryHostWeakReferences.FirstOrDefault(x => x.Key.Plugin.Name == file.FileName);
            if (plugin.Key != null)
            {
                Unload(plugin.Value, plugin.Key, plugin.Key.Path);
            }

            var fileName = Path.GetFileName(file.FileName);
            string pluginPath = $@"D:\c#\Plugins\{fileName}";

            if (System.IO.File.Exists(pluginPath))
            {
                var loader = DictionaryHostWeakReferences.FirstOrDefault(d => d.Key.Path == pluginPath);
                Unload(loader.Value, loader.Key, loader.Key.Path);

            }
            // Create new local file and copy contents of uploaded file
            using (var localFile = System.IO.File.OpenWrite(pluginPath))
            using (var uploadedFile = file.OpenReadStream())
            {
                uploadedFile.CopyTo(localFile);
            }

            _logger.LogInformation("File successfully uploaded");
            return RedirectToAction("Index", "Home");
        }
    }
}
