using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace SuricataToASFF
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private FileSystemWatcher _fsw;
        private string path = "/var/log/suricata";
        private string file = "eve.json";
        private object _lock;
        private long _position;
        private Mapper _mapper;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _mapper = new Mapper()
                .Init();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Worker started at: {time}", DateTimeOffset.Now);
            _fsw = new FileSystemWatcher(path, file);
            _fsw.Created += _fsw_Created;
            _fsw.Changed += _fsw_Changed;
            _fsw.Error += _fsw_Error;
            _fsw.EnableRaisingEvents = true;
            return base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            //_logger.LogInformation("Service Execute.");

            //while (!stoppingToken.IsCancellationRequested)
            //{
            //    _logger.LogInformation("Keep alive: {time}", DateTimeOffset.Now);
            //    await Task.Delay(5 * 60 * 1000, stoppingToken);
            //}

            //_logger.LogInformation("Service Execute stopped.");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Service");

            await base.StopAsync(cancellationToken);
        }

        private void _fsw_Error(object sender, ErrorEventArgs e)
        {
            _logger.LogInformation($"File error: {e.GetException().Message}");
        }
        private void _fsw_Changed(object sender, FileSystemEventArgs e)
        {
            _logger.LogInformation("File changed");

            List<Dictionary<string, JObject>> alerts;

            lock (_lock)
            {
                alerts = Mapper.GetFlowAlerts(path, file, ref _position);
                Console.WriteLine("----------------------------------------------");
                alerts.ForEach(alert => Console.WriteLine(Mapper.Map(alert)));
                Console.WriteLine("----------------------------------------------");
            }


        }
        private void _fsw_Created(object sender, FileSystemEventArgs e) => _logger.LogInformation("File created");
    }
}
