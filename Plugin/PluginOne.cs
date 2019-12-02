using System;
using System.ComponentModel.Composition;
using System.Dynamic;
using Interface;

namespace Plugin
{
    [Export(typeof(PluginOne))]
    public class PluginOne : IPlugin
    {
        public PluginOne()
        {
            Name = "Plugin 1 version 2";
        }
       
        public string GetMessage()
        {
            return Name;
        }

        public string Name { get; set; }


    }
}
