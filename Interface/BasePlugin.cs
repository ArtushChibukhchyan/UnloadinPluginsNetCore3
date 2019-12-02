using System;
using System.Collections.Generic;
using System.Text;
using Plugin;

namespace Interface
{
    public class BasePlugin:IPlugin
    {
        public BasePlugin()
        {
            
        }
        public BasePlugin(IPlugin plutinInstance)
        {
            this.Name = plutinInstance.Name;
            Id = plutinInstance.Id;
        }

        public Guid Id { get; set; }

        public string GetMessage()
        {
            return $"I am plugin {Name}"; ;
        }


        public string Name { get; set; }

    }
}
