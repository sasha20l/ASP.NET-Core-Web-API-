using System;
using System.Collections.Generic;

namespace MetricsManager.Responses
{
    public class DotNetMetricDto
    {
        public int Id { get; set; }
        public int AgentID { get; set; }
        public int Value { get; set; }
        public DateTimeOffset Time { get; set; }
    }
    public class SelectByTimePeriodDotNetMetricsResponse
    {
        public List<DotNetMetricDto> Metrics { get; set; }
    }
  }
