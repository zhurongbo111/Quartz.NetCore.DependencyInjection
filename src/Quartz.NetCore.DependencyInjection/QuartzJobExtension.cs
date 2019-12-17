﻿using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Quartz.NetCore.DependencyInjection
{
    public static class QuartzJobExtension
    {
        private const string DefaultJobGroup = "DefaultJobGroup";

        private const string DefaultTriggerGroup = "DefaultTriggerGroup";

        private const string DefaultSechedule = "DefaultSechedule";

        public static IServiceCollection AddQuartzJob<TJob>(this IServiceCollection services, ServiceLifetime jobLifetime = ServiceLifetime.Transient)
            where TJob : class, IJob
        {
            services.TryAddSingleton<IJobFactory, QuartzJobFactory>();
            services.TryAddSingleton<ISchedulerFactory, StdSchedulerFactory>();
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
            return services;
        }

        public static IServiceProvider UseQuartzJob<TJob>(this IServiceProvider applicationServices, Func<JobBuilder, IJobDetail> configJobDetail, string schedName) where TJob : IJob
        {
            var jobBuild = JobBuilder.Create<TJob>();
            var jobDetail = configJobDetail?.Invoke(jobBuild);
            var schedulerFactory = applicationServices.GetRequiredService<ISchedulerFactory>();
            IScheduler schedule = GetScheduler(schedulerFactory, schedName);
            schedule.JobFactory = applicationServices.GetRequiredService<IJobFactory>();
            schedule.AddJob(jobDetail, true);
            return applicationServices;
        }

        public static IServiceProvider UseQuartzJobWithTrigger<TJob>(this IServiceProvider applicationServices, Func<JobBuilder, IJobDetail> configJobDetail, Func<TriggerBuilder, ITrigger> configTrigger, string schedName)
            where TJob : IJob
        {
            var jobBuild = JobBuilder.Create<TJob>();
            var jobDetail = configJobDetail?.Invoke(jobBuild);

            var triggerBuild = TriggerBuilder.Create();
            var trigger = configTrigger?.Invoke(triggerBuild);

            var schedulerFactory = applicationServices.GetRequiredService<ISchedulerFactory>();
            IScheduler schedule = GetScheduler(schedulerFactory, schedName);
            schedule.ScheduleJob(jobDetail, trigger);
            schedule.JobFactory = applicationServices.GetRequiredService<IJobFactory>();
            return applicationServices;
        }


        public static IServiceProvider UseQuartzJobWithRepeat<TJob>(this IServiceProvider applicationServices, DateTime startTime, TimeSpan intervalTime)
            where TJob : IJob
        {
            Func<JobBuilder, IJobDetail> configJobDetail = jobBuilder => jobBuilder.WithIdentity(GetDefaultJobKey<TJob>()).Build();

            Func<TriggerBuilder, ITrigger> configTrigger = triggerBuild => triggerBuild.WithIdentity(GetDefaultTriggerKey<TJob>())
                .StartAt(startTime)
                .WithSimpleSchedule(
                   ssb => ssb.WithInterval(intervalTime)
                             .RepeatForever())
                .Build();
            applicationServices.UseQuartzJobWithTrigger<TJob>(configJobDetail, configTrigger, DefaultSechedule);
            return applicationServices;
        }

        public static IServiceProvider StartQuartzJob(this IServiceProvider applicationServices)
        {
            var schedulerFactory = applicationServices.GetRequiredService<ISchedulerFactory>();
            var allSchedules = schedulerFactory.GetAllSchedulers().Result;
            foreach (var schedule in allSchedules)
            {
                schedule.Start();
            }
            return applicationServices;
        }

        public static void StopQuartzJob(this IServiceProvider applicationServices, bool waitForJobsToComplete = true)
        {
            var schedulerFactory = applicationServices.GetRequiredService<ISchedulerFactory>();
            var allSchedules = schedulerFactory.GetAllSchedulers().Result;
            foreach (var schedule in allSchedules)
            {
                schedule.Shutdown(waitForJobsToComplete);
            }
        }

        public static void TriggerJob<TJob>(this ISchedulerFactory schedulerFactory, string schedName = DefaultSechedule) where TJob : IJob
        {
            var schedule = GetScheduler(schedulerFactory, schedName);
            schedule.TriggerJob(GetDefaultJobKey<TJob>());
        }

        private static JobKey GetDefaultJobKey<TJob>() where TJob : IJob
        {
            string name = typeof(TJob).Name;
            return JobKey.Create(name, DefaultJobGroup);
        }

        private static TriggerKey GetDefaultTriggerKey<TJob>() where TJob : IJob
        {
            string name = typeof(TJob).Name + "Trigger";
            return new TriggerKey(name, DefaultTriggerGroup);
        }

        private static IScheduler GetScheduler(ISchedulerFactory schedulerFactory, string schedName)
        {
            IScheduler schedule;
            if (schedName == DefaultSechedule)
            {
                schedule = schedulerFactory.GetScheduler().Result;
            }
            else
            {
                schedule = schedulerFactory.GetScheduler(schedName).Result;
            }
            return schedule;
        }
    }
}
