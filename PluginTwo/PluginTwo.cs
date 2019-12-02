using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Interface;
using Plugin;

namespace Plugin
{
    [Export(typeof(PluginTwo))]
    public class PluginTwo : IPlugin
    {
        public PluginTwo()
        {
            Name = "Plugin 2 version 2";
        }

        public string GetMessage()
        {
            return Name;
        }

        public string Name { get; set; }


    }
}
