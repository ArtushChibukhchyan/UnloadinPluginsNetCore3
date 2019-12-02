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
    public class PluginTwo : IPlugin
    {
        public PluginTwo()
        {
            Name = "Plugin 2 version 2";
        }

        private Guid _id = new Guid("577ae69a-0414-4e80-8705-dd86305d2d8e");

        public Guid Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string GetMessage()
        {
            return Name;
        }

        public string Name { get; set; }


    }
}
