using System;
using System.Collections.Generic;

namespace MetricsManager.Responses
{
    public class SelectByTimePeriodHddMetricsResponse
    {
        public List<HddMetricDto> Metrics { get; set; }
    }
    public class HddMetricDto
    {
        public int Id { get; set; }
        public int AgentID { get; set; }
        public int Value { get; set; }
        public DateTimeOffset Time { get; set; }
    }
}
