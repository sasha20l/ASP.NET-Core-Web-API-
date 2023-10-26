using Dapper;
using MetricsCommon.SQL_Settings;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace MetricsManager.DAL
{

    public interface IAgentsRepository
    {
        public void Create(AgentInfo item);
        public void Delete(int agentId);
        public IList<AgentInfo> GetAll();
        public AgentInfo GetAgentById(int agentId);
        public string FindUrl(string url);

    }
    public class AgentsRepository : IAgentsRepository
    {
        private readonly ILogger<AgentsRepository> _logger;
        public AgentsRepository(ILogger<AgentsRepository> logger, ISQLSettings sQLSettings)
        {
            _logger = logger;
        }

        public void Create(AgentInfo item)
        {
            try
            {
                using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
                {
                    connection.Execute($"INSERT INTO {SQLSettings.AgentsTable}({RegisteredAgentsFeilds.AgentId}," +
                        $" {RegisteredAgentsFeilds.AgentUrl}) VALUES(@AgentId, @AgentUrl)",
                        new { AgentId = item.AgentId, AgentUrl = item.AgentUrl});
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }
        public void Delete(int agentIdToDelete)
        {
            try
            {
                using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
                {
                    connection.Execute($"DELETE FROM {SQLSettings.AgentsTable} WHERE ({RegisteredAgentsFeilds.AgentId} = {agentIdToDelete})");                    
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
        }

        public IList<AgentInfo> GetAll()
        {
            try
            {
                using (var connection = new SQLiteConnection(SQLSettings.ConnectionString))
                {
                    return connection.Query<AgentInfo>($"SELECT * FROM {SQLSettings.AgentsTable}").ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
        public AgentInfo GetAgentById(int id)
        {
            try
            {
                using var connection = new SQLiteConnection(SQLSettings.ConnectionString);
                {
                    return connection.QueryFirstOrDefault<AgentInfo>($"SELECT * FROM {SQLSettings.AgentsTable} " +
                                                                     $"WHERE ({RegisteredAgentsFeilds.AgentId} = {id})");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
        public string FindUrl(string url)
        {
            try
            {
                using var connection = new SQLiteConnection(SQLSettings.ConnectionString);
                {
                    return connection.QueryFirstOrDefault<string>($"SELECT {RegisteredAgentsFeilds.AgentUrl} FROM {SQLSettings.AgentsTable} " +
                                                                  $"WHERE ({RegisteredAgentsFeilds.AgentUrl} = @AgentUrl)", 
                                                                  new { AgentUrl = url});
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
    }
}
