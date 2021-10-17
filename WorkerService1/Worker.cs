using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WorkerService1
{
    public sealed class WorkerService : IHostedService, IAsyncDisposable
    {
        private readonly ILogger<WorkerService> logger;
        private NamedPipeServerStream pipeServer;

        public WorkerService(ILogger<WorkerService> logger) => this.logger = logger;



        public Task StartAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation($"{nameof(WorkerService)} is running.");

            pipeServer = new NamedPipeServerStream("TestPipe", PipeDirection.InOut);

            logger.LogInformation("Waiting..");

            pipeServer.WaitForConnection();

            logger.LogInformation("Someone connected");


            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            logger.LogInformation($"{nameof(WorkerService)} is stopping.");


            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            if (pipeServer is IAsyncDisposable s)
            {
                await s.DisposeAsync();
            }

            pipeServer = null;
        }
    }
}
