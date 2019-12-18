using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quartz.NetCore.DependencyInjection.Test
{
    public class DemoJob : IJob, IDisposable
    {
        private readonly IHelloService helloService;

        public bool Disposed { get; set; }

        public string HelloResult { get; set; }

        public DemoJob(IHelloService helloService)
        {
            this.helloService = helloService;
        }

        public Task Execute(IJobExecutionContext context)
        {
            HelloResult = this.helloService.Say();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            this.Disposed = true;
        }
    }
}
