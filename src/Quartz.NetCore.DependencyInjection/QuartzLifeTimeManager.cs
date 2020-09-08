using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Quartz.NetCore.DependencyInjection
{
    public class QuartzLifeTimeManager
    {
        internal static List<JobDescription> JobDescriptions = new List<JobDescription>();

        private readonly ISchedulerFactory schedulerFactory;
        private readonly IJobFactory jobFactory;

        public QuartzLifeTimeManager(ISchedulerFactory schedulerFactory, IJobFactory jobFactory)
        {
            this.schedulerFactory = schedulerFactory;
            this.jobFactory = jobFactory;
        }


        public async Task Start()
        {
            var scheduler =  await this.schedulerFactory.GetScheduler();
            scheduler.JobFactory = jobFactory;
            foreach (var jobDescription in JobDescriptions)
            {
                await scheduler.ScheduleJob(jobDescription.JobDetail, jobDescription.JobTrigger);
            }
            await scheduler.Start();
        }

        public async Task Stop(bool waitForJobsToComplete = true)
        {
            var scheduler = await this.schedulerFactory.GetScheduler();
            await scheduler.Shutdown(waitForJobsToComplete);
        }
    }
}
