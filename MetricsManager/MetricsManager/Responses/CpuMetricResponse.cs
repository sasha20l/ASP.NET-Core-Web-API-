using System;
using System.Collections.Generic;

namespace MetricsManager.Responses
{
    public class CpuMetricDto
    {
        public int Id { get; set; }
        public int AgentID { get; set; }
        public int Value { get; set; }
        public DateTimeOffset Time { get; set; }
    }
   
    public class SelectByTimePeriodCpuMetricsResponse
    {
        public List<CpuMetricDto> Metrics { get; set; }
    }
}
