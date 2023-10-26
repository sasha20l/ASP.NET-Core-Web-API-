using Dapper;
using MetricsAgent.Models;
using MetricsCommon.SQL_Settings;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;

namespace MetricsAgent.DTO
{
    // маркировочный интерфейс
    // необходим, чтобы проверить работу репозитория на тесте-заглушке
    public interface INetworkMetricsRepository : IRepository<NetworkMetric>
    {
    }

    public class NetworkMetricsRepository : INetworkMetricsRepository
    {
        public void Create(NetworkMetric item)
        {
            try
            {
                using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
                {
                    connection.Execute($"INSERT INTO {Tables.NetworkMetrics}({AgentFields.Value}, {AgentFields.Time}) VALUES(@value, @time)",
                                                        new { value = item.Value, time = item.Time });
                }
            }
            catch (Exception) { }
        }
        public IList<NetworkMetric> GetAll()
        {
            using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
            {
                return connection.Query<NetworkMetric>($"SELECT * FROM {Tables.NetworkMetrics}").ToList();
            }
        }
        public IList<NetworkMetric> GetByTimePeriod(long getFromTime, long getToTime)
        {
            using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
            {
                return connection.Query<NetworkMetric>($"SELECT * FROM {Tables.NetworkMetrics} " +
                                                       $"WHERE ({AgentFields.Time} > @fromTime) AND ({AgentFields.Time} <= @toTime)",
                                                        new { fromTime = getFromTime, toTime = getToTime }).ToList();
            }
        }
    }
}
