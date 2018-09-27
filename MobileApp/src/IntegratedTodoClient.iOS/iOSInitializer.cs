using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Identity.Client;
using Prism;
using Prism.Ioc;

namespace IntegratedTodoClient.iOS
{
    public class iOSInitializer : IPlatformInitializer
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register Any Platform Specific Implementations that you cannot 
            // access from Shared Code
            containerRegistry.RegisterInstance(new UIParent(true));
        }
    }
}
