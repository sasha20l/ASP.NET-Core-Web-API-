using AutoMapper;
using MetricsManager.Client;
using MetricsManager.DAL;
using MetricsManager.Models;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MetricsManager.Jobs
{ 
    [DisallowConcurrentExecution]
    public class CpuMetricJob : IJob
    {
        // Инжектируем DI провайдер
        private readonly ICpuMetricsRepository _cpuMetricsRepository;
        private readonly IAgentsRepository _agentsRepository;
        private readonly ILogger<CpuMetricJob> _logger;
        private readonly IMetricsAgentClient _metricsAgentClient;
        private readonly IMapper _mapper;
        public CpuMetricJob(ICpuMetricsRepository cpuMetricsRepository, ILogger<CpuMetricJob> logger, 
                            IMetricsAgentClient metricsAgentClient, IMapper mapper, IAgentsRepository agentsRepository)
        {
            _cpuMetricsRepository = cpuMetricsRepository;
            _agentsRepository = agentsRepository;
            _logger = logger;
            _metricsAgentClient = metricsAgentClient;
            _mapper = mapper;
        }
        public Task Execute(IJobExecutionContext context)
        {
            // логируем, что мы пошли в соседний сервис
            _logger.LogInformation($"starting new request to metrics agent");
            // Получение списка зарегистрированных агентов
            var agents = _agentsRepository.GetAll();
            if (agents.Any())
            {
                foreach (var agent in agents)
                {
                    try
                    {
                        var metrics = _metricsAgentClient.GetAllCpuMetrics(new GetCpuMetricsApiRequestByTimePeriod
                        {
                            AgentUrl = agent.AgentUrl,
                            FromTime = _cpuMetricsRepository.GetLastRecordTimeByAgentId(agent.AgentId),
                            ToTime = DateTimeOffset.UtcNow
                        });
                        var metricForManagerDB = new List<CpuMetric>();
                        foreach (var metric in metrics.Metrics)
                        {
                            metricForManagerDB.Add(_mapper.Map<CpuMetric>(metric, id => metric.AgentID = agent.AgentId));
                        }
                        _cpuMetricsRepository.Create(metricForManagerDB);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex.Message);
                    }
                }
            }
            return Task.CompletedTask;
        }
    }
}
