using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz;
using Quartz.Impl;
using Quartz.NetCore.DependencyInjection;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Extensions.DependencyInjection
{
    public static partial class ServiceCollectionExtension
    {
        public static IServiceCollection ConfigQuartzJob<TJob>(this IServiceCollection services, Func<JobBuilder, IJobDetail> configJobDetail = null, Func<TriggerBuilder, ITrigger> configTrigger = null)
            where TJob : IJob
        {
            services.TryAddSingleton<IJobFactory, QuartzJobFactory>();
            services.TryAddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.TryAddSingleton<QuartzLifeTimeManager>();

            var jobBuild = JobBuilder.Create<TJob>();
            var jobDetail = configJobDetail?.Invoke(jobBuild);

            var triggerBuild = TriggerBuilder.Create();
            var trigger = configTrigger?.Invoke(triggerBuild);

            QuartzLifeTimeManager.JobDescriptions.Add(new JobDescription() { 
                JobDetail = jobDetail,
                JobTrigger = trigger,
                JobType = typeof(TJob)
            });

            return services;
        }

        public static IServiceProvider StartQuartzJobs(this IServiceProvider serviceProvider)
        {
            var starter = serviceProvider.GetRequiredService<QuartzLifeTimeManager>();

            starter.Start().GetAwaiter().GetResult();

            return serviceProvider;
        }

        public static IServiceProvider StopQuartzJobs(this IServiceProvider serviceProvider, bool waitForJobsToComplete = true)
        {
            var starter = serviceProvider.GetRequiredService<QuartzLifeTimeManager>();

            starter.Stop(waitForJobsToComplete).GetAwaiter().GetResult();

            return serviceProvider;
        }
    }
}
