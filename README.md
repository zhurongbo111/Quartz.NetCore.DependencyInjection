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


If you are in asp.net core 3.1 or later, you can just call:
```csharp
services.AutoStartQuartzJob()
```

else you need get `QuartzLifeTimeManager` instance from container, then call then `Start` method. Note: You should get instance at root service provider
