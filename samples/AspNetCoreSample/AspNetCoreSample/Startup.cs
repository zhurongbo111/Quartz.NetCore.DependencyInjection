using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Quartz;
using Quartz.NetCore.DependencyInjection;
using System;

namespace AspNetCoreSample
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.ConfigQuartzJob<DemoJob>(
                jobBuilder => jobBuilder.WithIdentity("DemoJobKey").Build(),
                //Run job every 10 seconds
                triggerBuild => triggerBuild.WithIdentity("DemoJobTriggerKey")
                                            .StartAt(DateTime.Now.AddSeconds(10))
                                            .WithSimpleSchedule(
                                                    ssb => ssb.WithInterval(TimeSpan.FromSeconds(10))
                                                    .RepeatForever())
                                            .Build());

            services.AutoStartQuartzJob();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime applicationLifetime)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Hello World!");
                });
            });
        }
    }
}
