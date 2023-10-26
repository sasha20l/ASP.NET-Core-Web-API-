using System;
using System.Collections.Generic;

namespace MetricsAgent.Responses
{
    public class CpuMetricDto
    {
        public DateTimeOffset Time { get; set; }
        public int Value { get; set; }
        public int Id { get; set; }
    }
    public class AllCpuMetricsResponse
    {
        public List<CpuMetricDto> Metrics { get; set; }
    }
    public class SelectByTimePeriodCpuMetricsResponse : AllCpuMetricsResponse
    {
    }
}
