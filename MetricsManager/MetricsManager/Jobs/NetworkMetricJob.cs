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
    public class NetworkMetricJob : IJob
    {
        // Инжектируем DI провайдер
        private readonly INetworkMetricsRepository _networkMetricsRepository;
        private readonly IAgentsRepository _agentsRepository;
        private readonly ILogger<NetworkMetricJob> _logger;
        private readonly IMetricsAgentClient _metricsAgentClient;
        private readonly IMapper _mapper;

        public NetworkMetricJob(INetworkMetricsRepository networkMetricsRepository, ILogger<NetworkMetricJob> logger,
                            IMetricsAgentClient metricsAgentClient, IMapper mapper, IAgentsRepository agentsRepository)
        {
            _networkMetricsRepository = networkMetricsRepository;
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
                        var metrics = _metricsAgentClient.GetAllNetworkMetrics(new GetNetworkMetricsApiRequestByTimePeriod
                        {
                            AgentUrl = agent.AgentUrl,
                            FromTime = _networkMetricsRepository.GetLastRecordTimeByAgentId(agent.AgentId),
                            ToTime = DateTimeOffset.UtcNow
                        });
                        var metricForManagerDB = new List<NetworkMetric>();
                        foreach (var metric in metrics.Metrics)
                        {
                            metricForManagerDB.Add(_mapper.Map<NetworkMetric>(metric, id => metric.AgentID = agent.AgentId));
                        }
                        _networkMetricsRepository.Create(metricForManagerDB);
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
