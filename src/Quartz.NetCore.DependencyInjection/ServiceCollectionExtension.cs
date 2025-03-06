using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz.Impl;
using Quartz.Spi;
using System;

namespace Quartz.NetCore.DependencyInjection
{
    public static partial class ServiceCollectionExtension
    {
        public static IServiceCollection ConfigQuartzJob<TJob>(this IServiceCollection services, Func<JobBuilder, IJobDetail> configJobDetail = null, Func<TriggerBuilder, ITrigger> configTrigger = null, ServiceLifetime jobLifetime = ServiceLifetime.Transient)
            where TJob : class, IJob
        {
            services.TryAddSingleton<IJobFactory, QuartzJobFactory>();
            services.TryAddSingleton<ISchedulerFactory, StdSchedulerFactory>();
            services.TryAddSingleton<QuartzLifeTimeManager>();

            switch (jobLifetime)
            {
                case ServiceLifetime.Singleton:
                    services.AddSingleton<TJob>();
                    break;
                case ServiceLifetime.Scoped:
                    services.AddScoped<TJob>();
                    break;
                case ServiceLifetime.Transient:
                    services.AddTransient<TJob>();
                    break;
            }

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

#if NETSTANDARD2_0
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
#endif

#if NET5_0_OR_GREATER

        private static bool hostServiceAdded = false;

        public static IServiceCollection ConfigQuartzJobAndAutoStart<TJob>(this IServiceCollection services, Func<JobBuilder, IJobDetail> configJobDetail = null, Func<TriggerBuilder, ITrigger> configTrigger = null, ServiceLifetime jobLifetime = ServiceLifetime.Transient)
            where TJob : class, IJob
        {
            services.ConfigQuartzJob<TJob>(configJobDetail, configTrigger, jobLifetime);

            services.AutoStartQuartzJob();

            return services;
        }

        public static IServiceCollection AutoStartQuartzJob(this IServiceCollection services)
        {
            if (!hostServiceAdded)
            {
                hostServiceAdded = true;
                services.AddHostedService<QuartzStartBackgroundService>();
            }

            return services;
        }
#endif
    }
}
