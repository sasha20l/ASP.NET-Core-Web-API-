
namespace MetricsManager.Client
{
    public interface IMetricsAgentClient
    {
        AllCpuMetricsApiRespons GetAllCpuMetrics(GetCpuMetricsApiRequestByTimePeriod request);
        AllDotNetMetricsApiRespons GetAllDotNetMetrics(GetDotNetMetricsApiRequestByTimePeriod request);
        AllHddMetricsApiRespons GetAllHddMetrics(GetHddMetricsApiRequestByTimePeriod request);
        AllNetworkMetricsApiRespons GetAllNetworkMetrics(GetNetworkMetricsApiRequestByTimePeriod request);
        AllRamMetricsApiRespons GetAllRamMetrics(GetRamMetricsApiRequestByTimePeriod request);


    }
}
