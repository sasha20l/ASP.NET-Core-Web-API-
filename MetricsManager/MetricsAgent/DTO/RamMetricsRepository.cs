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
    public interface IRamMetricsRepository : IRepository<RamMetric>
    {
    }

    public class RamMetricsRepository : IRamMetricsRepository
    {
        public void Create(RamMetric item)
        {
            try
            {
                using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
                {
                    connection.Execute($"INSERT INTO {Tables.RamMetrics}({AgentFields.Value}, {AgentFields.Time}) VALUES(@value, @time)",
                                                        new { value = item.Value, time = item.Time });
                }
            }
            catch (Exception) { }            
        }
        public IList<RamMetric> GetAll()
        {
            using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
            {
                return connection.Query<RamMetric>($"SELECT * FROM {Tables.RamMetrics}").ToList();
            }
        }
        public IList<RamMetric> GetByTimePeriod(long getFromTime, long getToTime)
        {
            using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
            {
                return connection.Query<RamMetric>($"SELECT * FROM {Tables.RamMetrics} " +
                                                   $"WHERE ({AgentFields.Time} > @fromTime) AND ({AgentFields.Time} <= @toTime)",
                                                    new { fromTime = getFromTime, toTime = getToTime }).ToList();
            }
        }
    }
}
