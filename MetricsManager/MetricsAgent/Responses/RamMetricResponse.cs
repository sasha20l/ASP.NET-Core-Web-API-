using System;
using System.Collections.Generic;

namespace MetricsAgent.Responses
{
    public class RamMetricDto
    {
        public DateTimeOffset Time { get; set; }
        public int Value { get; set; }
        public int Id { get; set; }
    }
    public class AllRamMetricsResponse
    {
        public List<RamMetricDto> Metrics { get; set; }
    }
    public class SelectByTimePeriodRamMetricsResponse : AllRamMetricsResponse
    {
    }
}
