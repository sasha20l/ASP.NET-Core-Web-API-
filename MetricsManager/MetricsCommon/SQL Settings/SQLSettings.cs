using System.Collections.Generic;

namespace MetricsCommon.SQL_Settings
{
    public enum Tables
    {
        CpuMetrics,
        DotNetMetrics,
        HddMetrics,
        NetworkMetrics,
        RamMetrics
    }
    public enum AgentFields
    {
        Id,
        Value,
        Time
    }
    public enum ManagerFields
    {
        Id,
        AgentId,
        Value,
        Time
    }
    public enum RegisteredAgentsFeilds
    {
        AgentId,
        AgentUrl
    }
    public class SQLSettings : ISQLSettings
    {
        private static readonly string connectionString = @"Data Source=metrics.db; Version=3;";
        private static readonly string agentsTable = "registeredagents";
        private readonly Dictionary<Tables, string> tablesDB = new()
        {
            { Tables.CpuMetrics, "cpumetrics" },
            { Tables.DotNetMetrics, "dotnetmetrics" },
            { Tables.HddMetrics, "hddmetrics" },
            { Tables.NetworkMetrics, "networkmetrics" },
            { Tables.RamMetrics, "rammetrics" },
        };
        private readonly Dictionary<AgentFields, string> agentFields = new()
        {
            { AgentFields.Id, "Id" },
            { AgentFields.Time, "Time" },
            { AgentFields.Value, "Value" },
        };
        private readonly Dictionary<ManagerFields, string> managerFields = new()
        {
            { ManagerFields.Id, "Id" },
            { ManagerFields.AgentId, "AgentId" },
            { ManagerFields.Time, "Time" },
            { ManagerFields.Value, "Value" },
        };
        private readonly Dictionary<RegisteredAgentsFeilds, string> registeredAgentsFeilds = new()
        {
            { RegisteredAgentsFeilds.AgentId, "AgentId" },
            { RegisteredAgentsFeilds.AgentUrl, "AgentUrl" },
        };
        public static string ConnectionString
        {
            get { return connectionString; }
        }
        public static string AgentsTable
        {
            get { return agentsTable; }
        }
        public string this[Tables key]
        {
            get { return tablesDB [key]; }
        }
        public string this[AgentFields key]
        {
            get { return agentFields[key]; }
        }
        public string this[ManagerFields key]
        {
            get { return managerFields[key]; }
        }
        public string this[RegisteredAgentsFeilds key]
        {
            get { return registeredAgentsFeilds[key]; }
        }
    }
}
