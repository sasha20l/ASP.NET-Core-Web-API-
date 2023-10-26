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
    public interface ICpuMetricsRepository : IRepository<CpuMetric>
    {
    }

    public class CpuMetricsRepository : ICpuMetricsRepository
    {
        
        public void Create(CpuMetric item)
        {
            try
            {
                using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
                {
                    connection.Execute($"INSERT INTO {Tables.CpuMetrics}({AgentFields.Value}, {AgentFields.Time}) VALUES(@value, @time)",
                                                        new { value = item.Value, time = item.Time });
                }
            }
            catch (Exception) { }          
        }

        public IList<CpuMetric> GetAll()
        {
            using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
            {
                return connection.Query<CpuMetric>($"SELECT * FROM {Tables.CpuMetrics}").ToList();
            }
        }   
        public IList<CpuMetric> GetByTimePeriod (long getFromTime, long getToTime)
        {
            using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
            {
                return connection.Query<CpuMetric>($"SELECT * FROM {Tables.CpuMetrics} " +
                                                   $"WHERE ({AgentFields.Time} > @fromTime) AND ({AgentFields.Time} <= @toTime)",
                                                    new { fromTime = getFromTime, toTime = getToTime }).ToList();
            }
        }
    }
}
