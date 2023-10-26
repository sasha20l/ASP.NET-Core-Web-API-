using MetricsManager.Responses;
using System.Collections.Generic;

namespace MetricsManager.Client
{
    public abstract class AllMetricsApiRespons<T>
    {
        public List<T> Metrics { get; set; }
    }
    public class AllCpuMetricsApiRespons : AllMetricsApiRespons<CpuMetricDto> 
    { 
    }    
    public class AllDotNetMetricsApiRespons : AllMetricsApiRespons<DotNetMetricDto>
    {
    }
    public class AllHddMetricsApiRespons : AllMetricsApiRespons<HddMetricDto>
    {
    }
    public class AllNetworkMetricsApiRespons : AllMetricsApiRespons<NetworkMetricDto>
    {
    }
    public class AllRamMetricsApiRespons : AllMetricsApiRespons<RamMetricDto>
    {
    }
}
