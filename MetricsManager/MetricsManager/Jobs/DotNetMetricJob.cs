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
    public class DotNetMetricJob : IJob
    {
        // Инжектируем DI провайдер
        private readonly IDotNetMetricsRepository _dotNetMetricsRepository;
        private readonly IAgentsRepository _agentsRepository;
        private readonly ILogger<DotNetMetricJob> _logger;
        private readonly IMetricsAgentClient _metricsAgentClient;
        private readonly IMapper _mapper;

        public DotNetMetricJob(IDotNetMetricsRepository dotNetMetricsRepository, ILogger<DotNetMetricJob> logger,
                            IMetricsAgentClient metricsAgentClient, IMapper mapper, IAgentsRepository agentsRepository)
        {
            _dotNetMetricsRepository = dotNetMetricsRepository;
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
                        var metrics = _metricsAgentClient.GetAllDotNetMetrics(new GetDotNetMetricsApiRequestByTimePeriod
                        {
                            AgentUrl = agent.AgentUrl,
                            FromTime = _dotNetMetricsRepository.GetLastRecordTimeByAgentId(agent.AgentId),
                            ToTime = DateTimeOffset.UtcNow
                        });
                        var metricForManagerDB = new List<DotNetMetric>();
                        foreach (var metric in metrics.Metrics)
                        {
                            metricForManagerDB.Add(_mapper.Map<DotNetMetric>(metric, id => metric.AgentID = agent.AgentId));
                        }
                        _dotNetMetricsRepository.Create(metricForManagerDB);
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
