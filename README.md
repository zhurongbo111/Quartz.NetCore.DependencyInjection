# Quartz.NetCore.DependencyInjection
The project is the extensions of Quartz for .net core project, it benefits you from DependencyInjection.

![nuget publish](https://github.com/zhurongbo111/Quartz.NetCore.DependencyInjection/workflows/nuget%20publish/badge.svg?branch=master)

Packages
--------

NuGet feed: https://www.nuget.org/packages/Quartz.NetCore.DependencyInjection/

| Package | NuGet Stable | NuGet Pre-release | Downloads |
| ------- | ------------ | ----------------- | --------- |
| [Quartz.NetCore.DependencyInjection](https://www.nuget.org/packages/Quartz.NetCore.DependencyInjection/) | [![Quartz.NetCore.DependencyInjection](https://img.shields.io/nuget/v/Quartz.NetCore.DependencyInjection.svg)](https://www.nuget.org/packages/Quartz.NetCore.DependencyInjection/) | [![Quartz.NetCore.DependencyInjection](https://img.shields.io/nuget/vpre/Quartz.NetCore.DependencyInjection.svg)](https://www.nuget.org/packages/Quartz.NetCore.DependencyInjection/) | [![Quartz.NetCore.DependencyInjection](https://img.shields.io/nuget/dt/Quartz.NetCore.DependencyInjection.svg)](https://www.nuget.org/packages/Quartz.NetCore.DependencyInjection/) |

Features
--------
Quartz.NetCore.DependencyInjection is a [NuGet library](https://www.nuget.org/packages/Quartz.NetCore.DependencyInjection) that you can add into your project that will benefit you from DependencyInjection for Quartz Job.

Examples
--------
please see the example:[samples](https://github.com/zhurongbo111/Quartz.NetCore.DependencyInjection/tree/master/samples/)

Usage
--------
First of all, add this namespace:
```csharp
using Quartz.NetCore.DependencyInjection;
```

In `ConfigureServices`, you need register jobs to container:
```csharp
    services.ConfigQuartzJob<DemoJob>(
                jobBuilder => jobBuilder.WithIdentity("DemoJobKey").Build(),
                //Run job every 10 seconds
                triggerBuild => triggerBuild.WithIdentity("DemoJobTriggerKey")
                                            .StartAt(DateTime.Now.AddSeconds(10))
                                            .WithSimpleSchedule(
                                                    ssb => ssb.WithInterval(TimeSpan.FromSeconds(10))
                                                    .RepeatForever())
                                            .Build());
```
Then in `Configure`, you need use this job and start:
```csharp
app.ApplicationServices.StartQuartzJobs()
```

At last if you want to stop the job when application is stopping, you can inject `IHostApplicationLifetime` to `Configure` method and  add follow:
```csharp
applicationLifetime.ApplicationStopping.Register(() => {
    app.ApplicationServices.StopQuartzJobs();}
);
```
