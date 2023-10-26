using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;

namespace MetricsManager.Client
{
    public class MetricsAgentClient : IMetricsAgentClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MetricsAgentClient> _logger;
        public MetricsAgentClient(HttpClient httpClient, ILogger<MetricsAgentClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public AllCpuMetricsApiRespons GetAllCpuMetrics(GetCpuMetricsApiRequestByTimePeriod request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get,
                $"{request.AgentUrl}/api/cpumetrics/from/{request.FromTime.UtcDateTime:O}/to/" +
                                                       $"{request.ToTime.UtcDateTime:O}");
            try
            {
                HttpResponseMessage response = _httpClient.SendAsync(httpRequest).Result;
                using var responseStream = response.Content.ReadAsStreamAsync().Result;
                return JsonSerializer.DeserializeAsync<AllCpuMetricsApiRespons>(responseStream, 
                                  new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
        public AllDotNetMetricsApiRespons GetAllDotNetMetrics(GetDotNetMetricsApiRequestByTimePeriod request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get,
                $"{request.AgentUrl}/api/dotnetmetrics/from/{request.FromTime.UtcDateTime:O}/to/" +
                                                       $"{request.ToTime.UtcDateTime:O}");
            try
            {
                HttpResponseMessage response = _httpClient.SendAsync(httpRequest).Result;
                using var responseStream = response.Content.ReadAsStreamAsync().Result;
                return JsonSerializer.DeserializeAsync<AllDotNetMetricsApiRespons>(responseStream,
                                  new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
        public AllHddMetricsApiRespons GetAllHddMetrics(GetHddMetricsApiRequestByTimePeriod request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get,
                $"{request.AgentUrl}/api/hddmetrics/from/{request.FromTime.UtcDateTime:O}/to/" +
                                                       $"{request.ToTime.UtcDateTime:O}");
            try
            {
                HttpResponseMessage response = _httpClient.SendAsync(httpRequest).Result;
                using var responseStream = response.Content.ReadAsStreamAsync().Result;
                return JsonSerializer.DeserializeAsync<AllHddMetricsApiRespons>(responseStream,
                                  new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
        public AllNetworkMetricsApiRespons GetAllNetworkMetrics(GetNetworkMetricsApiRequestByTimePeriod request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get,
                $"{request.AgentUrl}/api/networkmetrics/from/{request.FromTime.UtcDateTime:O}/to/" +
                                                       $"{request.ToTime.UtcDateTime:O}");
            try
            {
                HttpResponseMessage response = _httpClient.SendAsync(httpRequest).Result;
                using var responseStream = response.Content.ReadAsStreamAsync().Result;
                return JsonSerializer.DeserializeAsync<AllNetworkMetricsApiRespons>(responseStream,
                                  new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
        public AllRamMetricsApiRespons GetAllRamMetrics(GetRamMetricsApiRequestByTimePeriod request)
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get,
                $"{request.AgentUrl}/api/rammetrics/from/{request.FromTime.UtcDateTime:O}/to/" +
                                                       $"{request.ToTime.UtcDateTime:O}");
            try
            {
                HttpResponseMessage response = _httpClient.SendAsync(httpRequest).Result;
                using var responseStream = response.Content.ReadAsStreamAsync().Result;
                return JsonSerializer.DeserializeAsync<AllRamMetricsApiRespons>(responseStream,
                                  new JsonSerializerOptions { PropertyNameCaseInsensitive = true }).Result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
            }
            return null;
        }
    }
}
