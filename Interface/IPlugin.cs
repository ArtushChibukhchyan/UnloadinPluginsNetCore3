﻿using System;
using System.ComponentModel.Composition;

namespace Plugin
{

    // The Interface is defined in an Assembly shared between the host and the plugin.
    // That makes calling functions from the plugin easier (without having to use reflection
    // to invoke all of the functions - we just use reflection once to get the Interface)
    // NOTE:
    // The Assembly that defines the Interface must be loaded into the AssemblyLoadContext 
    // of the host only. If it got loaded twice - once into the AssemblyLoadContext in which 
    // the plugin is loaded and once into the default AssemblyLoadContext where the host is loaded, 
    // the Interface would become two different types and it would not be possible to
    // use Interface instance created on the plugin side on the host side
    public interface IPlugin 
    {
         string GetMessage();
        string Name { get; set; }

    }
}
