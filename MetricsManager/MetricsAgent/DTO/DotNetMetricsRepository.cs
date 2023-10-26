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
    public interface IDotNetMetricsRepository : IRepository<DotNetMetric>
    {

    }
    public class DotNetMetricsRepository : IDotNetMetricsRepository
    {
        /// <summary>
        /// Добавление в базу данных новой метрики .Net 
        /// (счетчик текущего объема памяти, выделенной в килобайтах для куч сборки мусора) 
        /// </summary>
        /// <param name="item">Объект класса DotNetMetric</param>
        public void Create(DotNetMetric item)
        {
            try
            {
                using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
                {
                    connection.Execute($"INSERT INTO {Tables.DotNetMetrics}({AgentFields.Value}, {AgentFields.Time}) VALUES(@value, @time)",
                                                        new { value = item.Value, time = item.Time });
                }
            }
            catch (Exception) { }
        }
        public IList<DotNetMetric> GetAll()
        {
            using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
            {
                return connection.Query<DotNetMetric>($"SELECT * FROM {Tables.DotNetMetrics}").ToList();
            }            
        }
        public IList<DotNetMetric> GetByTimePeriod(long getFromTime, long getToTime)
        {
            using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
            {
                return connection.Query<DotNetMetric>($"SELECT * FROM {Tables.DotNetMetrics} " +
                                                      $"WHERE ({AgentFields.Time} > @fromTime) AND ({AgentFields.Time} <= @toTime)",
                                                        new { fromTime = getFromTime, toTime = getToTime }).ToList();
            }
        }
    }
}
