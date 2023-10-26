using AutoFixture;
using AutoMapper;
using MetricsCommon;
using MetricsManager;
using MetricsManager.Controllers;
using MetricsManager.DAL;
using MetricsManager.Models;
using MetricsManager.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace MetricsManagerTests
{
    public class CpuMetricsControllerUnitTest
    {
        private readonly CpuMetricsController _controller;
        private readonly Mock<ICpuMetricsRepository> _mockRepository;
        private readonly Mock<ILogger<CpuMetricsController>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly List<CpuMetric> _initialData;
        private const Percentile percentile = Percentile.P99;

        public CpuMetricsControllerUnitTest()
        {
            _mockRepository = new Mock<ICpuMetricsRepository>();
            _mockLogger = new Mock<ILogger<CpuMetricsController>>();
            _mapper = new MapperConfiguration(mp => mp.AddProfile(new MapperProfile())).CreateMapper();
            _controller = new CpuMetricsController(_mockLogger.Object, _mockRepository.Object, _mapper);
            _initialData = new Fixture().Create<List<CpuMetric>>();
        }

        [Fact]
        public void GetMetricsFromAgent_ShouldCall_GetByTimePeriodByAgentId_From_Repository()
        {
            _mockRepository.Setup(repository => repository
                           .GetByTimePeriodByAgentId(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<long>()))
                           .Returns(_initialData).Verifiable();
            var result = (OkObjectResult)_controller.GetMetricsFromAgent(It.IsAny<int>(), 
                                                                         It.IsAny<DateTimeOffset>(), 
                                                                         It.IsAny<DateTimeOffset>());
            var actualResult = ((SelectByTimePeriodCpuMetricsResponse)result.Value).Metrics;
            _mockRepository.Verify(repository => repository.GetByTimePeriodByAgentId(It.IsAny<int>(),
                                                                                     It.IsAny<long>(),
                                                                                     It.IsAny<long>()),
                                                                                     Times.AtMostOnce());
            for (int i = 0; i < _initialData.Count; i++)
            {
                Assert.Equal(_initialData[i].Value, actualResult[i].Value);
                Assert.Equal(_initialData[i].Id, actualResult[i].Id);
                Assert.Equal(_initialData[i].Time, actualResult[i].Time.ToUnixTimeSeconds());
            }
        }

        [Fact]
        public void GetMetricsByPercentileFromAgent_ShouldCall_GetByTimePeriodByAgentId_From_Repository()
        {
            _mockRepository.Setup(repository => repository
                           .GetByTimePeriodByAgentId(It.IsAny<int>(), It.IsAny<long>(), It.IsAny<long>()))
                           .Returns(_initialData).Verifiable();
            var result = (OkObjectResult)_controller.GetMetricsByPercentileFromAgent(It.IsAny<int>(),
                                                                         It.IsAny<DateTimeOffset>(),
                                                                         It.IsAny<DateTimeOffset>(),
                                                                         percentile);
            var actualResult = ((int)result.Value);
            _mockRepository.Verify(repository => repository.GetByTimePeriodByAgentId(It.IsAny<int>(),
                                                                                     It.IsAny<long>(),
                                                                                     It.IsAny<long>()),
                                                                                     Times.AtMostOnce());
            var orderedMetrics = _initialData.OrderBy(m => m.Value);
            var initialPersentile = orderedMetrics.ElementAt((int)(orderedMetrics.Count()*0.99)).Value;
            Assert.Equal(initialPersentile, actualResult);
        }

        [Fact]
        public void GetMetricsFromAllCluster_ShouldCall_GetByTimePeriodFromAllAgents_From_Repository()
        {
            _mockRepository.Setup(repository => repository
                           .GetByTimePeriodFromAllAgents(It.IsAny<long>(), It.IsAny<long>()))
                           .Returns(_initialData).Verifiable();
            var result = (OkObjectResult)_controller.GetMetricsFromAllCluster(It.IsAny<DateTimeOffset>(),
                                                                              It.IsAny<DateTimeOffset>());
            var actualResult = ((SelectByTimePeriodCpuMetricsResponse)result.Value).Metrics;
            _mockRepository.Verify(repository => repository.GetByTimePeriodFromAllAgents(It.IsAny<long>(),
                                                                                         It.IsAny<long>()),
                                                                                         Times.AtMostOnce());
            for (int i = 0; i < _initialData.Count; i++)
            {
                Assert.Equal(_initialData[i].Value, actualResult[i].Value);
                Assert.Equal(_initialData[i].Id, actualResult[i].Id);
                Assert.Equal(_initialData[i].Time, actualResult[i].Time.ToUnixTimeSeconds());
            }
        }

        [Fact]
        public void GetMetricsByPercentileFromAllCluster_ShouldCall_GetByTimePeriodFromAllAgents_From_Repository()
        {
            _mockRepository.Setup(repository => repository
                           .GetByTimePeriodFromAllAgents(It.IsAny<long>(), It.IsAny<long>()))
                           .Returns(_initialData).Verifiable();
            var result = (OkObjectResult)_controller.GetMetricsByPercentileFromAllCluster(It.IsAny<DateTimeOffset>(),
                                                                                          It.IsAny<DateTimeOffset>(),
                                                                                          percentile);
            var actualResult = ((int)result.Value);
            _mockRepository.Verify(repository => repository.GetByTimePeriodFromAllAgents(It.IsAny<long>(),
                                                                                         It.IsAny<long>()),
                                                                                         Times.AtMostOnce());
            var orderedMetrics = _initialData.OrderBy(m => m.Value);
            var initialPersentile = orderedMetrics.ElementAt((int)(orderedMetrics.Count() * 0.99)).Value;
            Assert.Equal(initialPersentile, actualResult);
        }
    }
}

