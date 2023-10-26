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
    public class RamMetricJob : IJob
    {
        // Инжектируем DI провайдер
        private readonly IRamMetricsRepository _ramMetricsRepository;
        private readonly IAgentsRepository _agentsRepository;
        private readonly ILogger<RamMetricJob> _logger;
        private readonly IMetricsAgentClient _metricsAgentClient;
        private readonly IMapper _mapper;

        public RamMetricJob(IRamMetricsRepository ramMetricsRepository, ILogger<RamMetricJob> logger,
                            IMetricsAgentClient metricsAgentClient, IMapper mapper, IAgentsRepository agentsRepository)
        {
            _ramMetricsRepository = ramMetricsRepository;
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
                        var metrics = _metricsAgentClient.GetAllRamMetrics(new GetRamMetricsApiRequestByTimePeriod
                        {
                            AgentUrl = agent.AgentUrl,
                            FromTime = _ramMetricsRepository.GetLastRecordTimeByAgentId(agent.AgentId),
                            ToTime = DateTimeOffset.UtcNow
                        });
                        var metricForManagerDB = new List<RamMetric>();
                        foreach (var metric in metrics.Metrics)
                        {
                            metricForManagerDB.Add(_mapper.Map<RamMetric>(metric, id => metric.AgentID = agent.AgentId));
                        }
                        _ramMetricsRepository.Create(metricForManagerDB);
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
