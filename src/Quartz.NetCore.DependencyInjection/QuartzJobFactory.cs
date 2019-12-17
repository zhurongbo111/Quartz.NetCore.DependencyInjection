using Microsoft.Extensions.DependencyInjection;
using Quartz.Spi;
using System;
using System.Collections.Concurrent;
using System.Reflection;

namespace Quartz.NetCore.DependencyInjection
{
    public class QuartzJobFactory : IJobFactory
    {
        private readonly IServiceProvider serviceProvider;

        private ConcurrentDictionary<string, IServiceScope> ServiceScopeCache = new ConcurrentDictionary<string, IServiceScope>();

        public QuartzJobFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            Type jobType = bundle.JobDetail.JobType;
            if (jobType.IsDefined(typeof(NoScopeInstanceAttribute)))
            {
                return this.serviceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
            }
            else
            {
                var serviceScope = this.serviceProvider.CreateScope();
                IJob job = serviceScope.ServiceProvider.GetRequiredService(bundle.JobDetail.JobType) as IJob;
                ServiceScopeCache.TryAdd(GetJobKey(job), serviceScope);
                return job;
            }
        }

        public void ReturnJob(IJob job)
        {
            if (ServiceScopeCache.TryGetValue(GetJobKey(job), out var serviceScope))
            {
                serviceScope.Dispose();
            }
            else
            {
                var disposable = job as IDisposable;
                disposable?.Dispose();
            }
        }


        private string GetJobKey(IJob job)
        {
            return job.GetType().Name + job.GetHashCode();
        }
    }
}
