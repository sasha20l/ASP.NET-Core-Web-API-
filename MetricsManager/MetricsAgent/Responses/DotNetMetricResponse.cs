using System;
using System.Collections.Generic;

namespace MetricsAgent.Responses
{
    public class DotNetMetricDto
    {
        public DateTimeOffset Time { get; set; }
        public int Value { get; set; }
        public int Id { get; set; }
    }
    public class AllDotNetMetricsResponse
    {
        public List<DotNetMetricDto> Metrics { get; set; }
    }
    public class SelectByTimePeriodDotNetMetricsResponse : AllDotNetMetricsResponse
    { 
    }
}
