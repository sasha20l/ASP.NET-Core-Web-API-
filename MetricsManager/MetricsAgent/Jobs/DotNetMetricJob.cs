using MetricsAgent.DTO;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace MetricsAgent.Jobs
{
    public class DotNetMetricJob : IJob
    {
        private readonly IDotNetMetricsRepository _repository;
        private readonly PerformanceCounter _dotNetCounter;

        public DotNetMetricJob(IDotNetMetricsRepository repository)
        {
            _repository = repository;
            _dotNetCounter = new PerformanceCounter(".NET CLR Memory", "# Bytes in all heaps", "_Global_"); 
        }
        public Task Execute(IJobExecutionContext context)
        {
            var allHeapSizeInKBytes = Convert.ToInt32(_dotNetCounter.NextValue()/1024);
            var time = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _repository.Create(new Models.DotNetMetric { Time = time, Value = allHeapSizeInKBytes });
            return Task.CompletedTask;
        }
    }
}
