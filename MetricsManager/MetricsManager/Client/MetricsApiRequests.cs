using System;

namespace MetricsManager.Client
{
    public abstract class GetMetricsApiRequestByTimePeriod
    {
        public string AgentUrl { get; set; }
        public DateTimeOffset FromTime { get; set; }
        public DateTimeOffset ToTime { get; set; }
    }
    public class GetCpuMetricsApiRequestByTimePeriod : GetMetricsApiRequestByTimePeriod
    { 
    }
    public class GetDotNetMetricsApiRequestByTimePeriod : GetMetricsApiRequestByTimePeriod
    {
    }
    public class GetHddMetricsApiRequestByTimePeriod : GetMetricsApiRequestByTimePeriod
    {
    }
    public class GetNetworkMetricsApiRequestByTimePeriod : GetMetricsApiRequestByTimePeriod
    {
    }
    public class GetRamMetricsApiRequestByTimePeriod : GetMetricsApiRequestByTimePeriod
    {
    }
}
