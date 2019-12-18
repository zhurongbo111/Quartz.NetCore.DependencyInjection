using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Moq;
using NUnit.Framework;
using Quartz.Spi;
using System;
using System.Threading.Tasks;

namespace Quartz.NetCore.DependencyInjection.Test
{

    public class QuartzJobFactoryTest
    {
        private IServiceProvider serviceProvider;

        [SetUp]
        public void TestInit()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddTransient<IHelloService, HelloService>();
            services.TryAddSingleton<IJobFactory, QuartzJobFactory>();
            services.AddTransient<DemoJob>();

            this.serviceProvider = services.BuildServiceProvider();
        }

        [Test]
        public async Task FactoryCreateJobInstance()
        {
            var jobFactory = this.serviceProvider.GetRequiredService<IJobFactory>();

            Mock<IJobDetail> jobdetail = new Mock<IJobDetail>();
            jobdetail.Setup(m => m.JobType).Returns(typeof(DemoJob));

            TriggerFiredBundle bundle = new TriggerFiredBundle(jobdetail.Object, new Mock<IOperableTrigger>().Object,
                new Mock<ICalendar>().Object,false, DateTimeOffset.Now,null,null,null);
  
            Mock<IScheduler> scheduler = new Mock<IScheduler>();
            DemoJob job = jobFactory.NewJob(bundle, scheduler.Object) as DemoJob;
            await job.Execute(null);
            Assert.AreEqual("Hello", job.HelloResult);
            Assert.IsFalse(job.Disposed);
            jobFactory.ReturnJob(job);
            Assert.IsTrue(job.Disposed);
        }
    }
}
