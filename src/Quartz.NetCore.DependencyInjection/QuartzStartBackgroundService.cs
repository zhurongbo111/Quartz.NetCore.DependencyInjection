#if NET5_0 || NETCOREAPP3_1
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Quartz.NetCore.DependencyInjection
{
    
    public class QuartzStartBackgroundService : IHostedService
    {
        private readonly QuartzLifeTimeManager quartzLifeTimeManager;

        public QuartzStartBackgroundService(QuartzLifeTimeManager quartzLifeTimeManager)
        {
            this.quartzLifeTimeManager = quartzLifeTimeManager;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await this.quartzLifeTimeManager.Start();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await this.quartzLifeTimeManager.Stop(true);
        }
    }
}
#endif
