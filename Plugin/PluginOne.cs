using System;
using Plugin;

namespace PluginOne
{
    public class PluginOne : IPlugin
    {
        public PluginOne()
        {
            Name = "Plugin 1 version 2";
        }
        private Guid _id = new Guid("14d65944-99ae-4fcd-8352-1c51c347bb07");

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
