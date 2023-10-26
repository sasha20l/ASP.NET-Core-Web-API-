using AutoMapper;
using MetricsManager.Models;
using MetricsManager.Responses;
using System;

namespace MetricsManager
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CpuMetricDto, CpuMetric>().ForMember(dbModel => dbModel.Time,
                                              o => o.MapFrom(t => t.Time.ToUnixTimeSeconds()));
            CreateMap<CpuMetric, CpuMetricDto>().ForMember(tm => tm.Time,
                                time => time.MapFrom(t => DateTimeOffset.FromUnixTimeSeconds(t.Time)));
            CreateMap<DotNetMetricDto, DotNetMetric>().ForMember(dbModel => dbModel.Time,
                                              o => o.MapFrom(t => t.Time.ToUnixTimeSeconds()));
            CreateMap<DotNetMetric, DotNetMetricDto>().ForMember(tm => tm.Time,
                                time => time.MapFrom(t => DateTimeOffset.FromUnixTimeSeconds(t.Time)));
            CreateMap<HddMetricDto, HddMetric>().ForMember(dbModel => dbModel.Time,
                                              o => o.MapFrom(t => t.Time.ToUnixTimeSeconds()));
            CreateMap<HddMetric, HddMetricDto>().ForMember(tm => tm.Time,
                                time => time.MapFrom(t => DateTimeOffset.FromUnixTimeSeconds(t.Time)));
            CreateMap<NetworkMetricDto, NetworkMetric>().ForMember(dbModel => dbModel.Time,
                                              o => o.MapFrom(t => t.Time.ToUnixTimeSeconds()));
            CreateMap<NetworkMetric, NetworkMetricDto>().ForMember(tm => tm.Time,
                                time => time.MapFrom(t => DateTimeOffset.FromUnixTimeSeconds(t.Time)));
            CreateMap<RamMetricDto, RamMetric>().ForMember(dbModel => dbModel.Time,
                                              o => o.MapFrom(t => t.Time.ToUnixTimeSeconds()));
            CreateMap<RamMetric, RamMetricDto>().ForMember(tm => tm.Time,
                                time => time.MapFrom(t => DateTimeOffset.FromUnixTimeSeconds(t.Time)));
        }
    }
}
