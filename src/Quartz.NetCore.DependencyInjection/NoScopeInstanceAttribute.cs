using System;
using System.Collections.Generic;
using System.Text;

namespace Quartz.NetCore.DependencyInjection
{
    /// <summary>
    /// By default, when create any job instance, the job is created with a scoped service provider.
    /// If a job is marked with this attribute, the job is created with the root service provider 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class NoScopeInstanceAttribute : Attribute
    {
    }
}
