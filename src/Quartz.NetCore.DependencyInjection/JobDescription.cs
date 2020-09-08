using System;
using System.Collections.Generic;
using System.Text;

namespace Quartz.NetCore.DependencyInjection
{
    internal class JobDescription
    {
        public IJobDetail JobDetail { get; set; }

        public ITrigger JobTrigger { get; set; }

        public Type JobType { get; set; }
    }
}
