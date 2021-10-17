using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WorkerService1;

using IHost host = Host.CreateDefaultBuilder(args)
        .UseWindowsService(options =>
        {
            options.ServiceName = ".NET Joke Service";
        })
        .ConfigureServices(services =>
        {
            services.AddHostedService<WorkerService>();
        })
        .Build();

await host.RunAsync();