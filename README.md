# Template_NET_CORE_3_WORKER
An opinionated service worker template for creating services that communicate via MassTransit (RabbitMQ or other transports) in a distributed environment.

# Idea
When building distributed applications certain base elements (aka. boilerplate code) are used all the time. In order to minimize work and create a consistent approach a template can improve the situation a lot.  
The template tries to seperate concerns whenever possible and useful. In some case this leads to more files than necessary and might seem to add unneeded complexcity (especially when looking at the sample), but for more complex applications it is quite useful.  
Dependency Injection is used whenever possible. Logging will use Microsofts Logging Extentions and a lot of configuration is moved to appsettings.json.  
For larger applications it might be useful to move services/domains to seperate projects.  
  
MassTransit is used to create an abstraction to RabbitMQ, which gives us the ability to change the message bus a later time. Several transports for i.e. Amazon SQS, Azure Service Bus, ... are available.

# Used Tools & Frameworks

* [.NET Core 3.1](https://dotnet.microsoft.com/download/dotnet-core)
* [Autofac](https://autofac.org/) | Dependency Injection Framework
* [Serilog](https://serilog.net/) | Logging Framework
* [MassTransit](https://masstransit-project.com/) | Distributed Application Framework
* [Quartz.NET](https://www.quartz-scheduler.net/) | Job Scheduler
* [xUnit](https://xunit.net/) and [Moq](https://github.com/Moq/moq4/wiki/Quickstart) | Unit testing
* [RabbitMQ](https://www.rabbitmq.com/) | Message Broker

The project uses additional nuget packages that extend the functionality of the above mentioned packages and allow all packages to interact and work together within the solution.

# Structure / Folders
```text
│   App.config
│   appsettings.json
│   Program.cs
│   Template_NET_CORE_3_WORKER.CoreService.csproj
│
├───AutofacModules
│       MassTransitRabbitModule.cs
│       QuartzJobModule.cs
│
├───HostedServices
│       LifeTimeEventService.cs
│       SampleQuartzService.cs
│
├───MassTransitConsumers
│       SampleConsumer.cs
│
├───Models
│   │   LifeTimeState.cs
│   │   SampleRequest.cs
│   │
│   └───Configuration
│           MassTransitRabbitConfig.cs
│           SampleQuartzServiceOptions.cs
│
└───QuartzJobs
        SampleJob.cs
```

`AutofacModules` contains all modules that get automatically loaded via configuration.  
`HostedServices` contains all IHostedService implementations. Need to be registered manually in program.cs.  
`MassTransitConsumers` contains all MassTransit consumers. Need to be configured in 'MassTransitRabbitModule.cs' in order to match a ReceiveEndpoint.  
`QuartzJobs` contains all IJob implementations which also need to be configured (trigger, scheduler, ...) in a matching HostingService.  

# Building the template
The template will be packed to a nuget package which can be installed via dotnet tooling. The package contains a project file "TemplatePack.csproj" which can be used to build the nuget package.

```dotnetcli
  dotnet pack TemplatePack.csproj
  ```

# Install, remove and use template
## Installing template

Use the [dotnet new -i|--install] command to install a package.

### To install template from a NuGet package feed (which is available in your configured package sources)

Use the NuGet package identifier to install a template package.

```dotnetcli
dotnet new -i JanKnipp.Templates.NetCore3.Worker.RabbitMQ
```

### To install template from a local nupkg file

Provide the path to JanKnipp.Templates.NetCore3.Worker.RabbitMQ.*.nupkg NuGet package file. Please specify the exact file name of the version you want to install.

```dotnetcli
dotnet new -i JanKnipp.Templates.NetCore3.Worker.RabbitMQ.1.0.0.nupkg
```

## Uninstalling template

Use the [dotnet new -u|--uninstall] command to uninstall package.

If the package was installed by either a NuGet feed or by a *.nupkg* file directly, provide the identifier.

```dotnetcli
dotnet new -u JanKnipp.Templates.NetCore3.Worker.RabbitMQ
```

## Create a project using a custom template

After the template is installed, use the template by executing the `dotnet new core3-mq-service` command as you would with any other pre-installed template. In order to change the solution/project name supply the name paramater `-n` and a name for the service. If you do not provide a name the template will automatically select the name of the folder in which the `dotnet new` command has been executed in as the name of the solution.

```dotnetcli
dotnet new core3-mq-service -n MyCustomName
```
