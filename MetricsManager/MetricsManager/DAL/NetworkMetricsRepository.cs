using Dapper;
using MetricsCommon.SQL_Settings;
using MetricsManager.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace MetricsManager.DAL
{
    public interface INetworkMetricsRepository : IRepository<NetworkMetric>
    {
    }
    public class NetworkMetricsRepository : INetworkMetricsRepository
    {
        private readonly ILogger<NetworkMetricsRepository> _logger;

        public NetworkMetricsRepository(ILogger<NetworkMetricsRepository> logger)
        {
            _logger = logger;
        }
        public void Create(List<NetworkMetric> metrics)
        {
            using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                    try
                    {
                        foreach (var metric in metrics)
                        {
                            connection.ExecuteAsync($"INSERT INTO {Tables.NetworkMetrics}" +
                                $"({ManagerFields.AgentId}, {ManagerFields.Value}, {ManagerFields.Time}) " +
                                $"VALUES(@agentid, @value, @time)",
                            new
                            {
                                agentid = metric.AgentID,
                                value = metric.Value,
                                time = metric.Time
                            });
                        }
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        _logger.LogError(ex.Message);
                    }
            }
        }
        public IList<NetworkMetric> GetByTimePeriodByAgentId(int requestedAgent, long getFromTime, long getToTime)
        {
            try
            {
                using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
                {
                    return connection.Query<NetworkMetric>($"SELECT * FROM {Tables.NetworkMetrics} " +
                        $"WHERE ({ManagerFields.AgentId} = @agentId) " +
                        $"AND ({ManagerFields.Time} >= @fromTime) AND ({ManagerFields.Time} <= @toTime)",
                        new { agentId = requestedAgent, fromTime = getFromTime, toTime = getToTime}).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
        public IList<NetworkMetric> GetByTimePeriodFromAllAgents(long getFromTime, long getToTime)
        {
            try
            {
                using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
                {
                    return connection.Query<NetworkMetric>($"SELECT * FROM {Tables.NetworkMetrics} " +
                        $"WHERE ({ManagerFields.Time} >= @fromTime) AND ({ManagerFields.Time} <= @toTime)",
                        new { fromTime = getFromTime, toTime = getToTime }).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
        public DateTimeOffset GetLastRecordTimeByAgentId (int agentId)
        {
            try
            {
                using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
                {
                    var res = connection.QuerySingleOrDefault<long>($"SELECT MAX({ManagerFields.Time}) " +
                               $"FROM {Tables.NetworkMetrics} " + $"WHERE ({ManagerFields.AgentId} = {agentId})");    
                    return DateTimeOffset.FromUnixTimeSeconds(res);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return DateTimeOffset.FromUnixTimeSeconds(0);
        }
    }
}

