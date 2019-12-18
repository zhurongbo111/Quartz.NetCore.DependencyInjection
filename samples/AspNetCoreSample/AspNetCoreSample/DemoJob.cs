using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AspNetCoreSample
{
    public class DemoJob : IJob
    {
        private readonly ILogger<DemoJob> logger;

        public DemoJob(ILogger<DemoJob> logger)
        {
            this.logger = logger;
        }
        public Task Execute(IJobExecutionContext context)
        {
            this.logger.LogInformation("Run DemoJob Execute");
            return Task.CompletedTask;
        }
    }
}
