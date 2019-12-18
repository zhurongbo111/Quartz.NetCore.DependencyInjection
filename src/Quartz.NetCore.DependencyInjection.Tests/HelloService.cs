using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Quartz.NetCore.DependencyInjection.Test
{
    public class HelloService : IHelloService
    {
        public string Say()
        {
            return "Hello";
        }
    }
}
