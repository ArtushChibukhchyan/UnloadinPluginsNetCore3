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
        public BasePlugin(IPlugin plugin)
        {
            this.Name = plugin.Name;
            Id = plugin.Id;
        }

        public Guid Id { get; set; }

        public string GetMessage()
        {
            return $"I am plugin {Name}"; ;
        }


        public string Name { get; set; }

    }
}
