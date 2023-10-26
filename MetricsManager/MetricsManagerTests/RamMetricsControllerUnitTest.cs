using AutoFixture;
using AutoMapper;
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
using Xunit;

namespace MetricsManagerTests
{
    public class RamMetricsControllerUnitTest
    {
        private readonly RamMetricsController _controller;
        private readonly Mock<IRamMetricsRepository> _mockRepository;
        private readonly Mock<ILogger<RamMetricsController>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly List<RamMetric> _initialData;
        public RamMetricsControllerUnitTest()
        {
            _mockRepository = new Mock<IRamMetricsRepository>();
            _mockLogger = new Mock<ILogger<RamMetricsController>>();
            _mapper = new MapperConfiguration(mp => mp.AddProfile(new MapperProfile())).CreateMapper();
            _controller = new RamMetricsController(_mockLogger.Object, _mockRepository.Object, _mapper);
            _initialData = new Fixture().Create<List<RamMetric>>();
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
            var actualResult = ((SelectByTimePeriodRamMetricsResponse)result.Value).Metrics;
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
        public void GetMetricsFromAllCluster_ShouldCall_GetByTimePeriodFromAllAgents_From_Repository()
        {
            _mockRepository.Setup(repository => repository
                           .GetByTimePeriodFromAllAgents(It.IsAny<long>(), It.IsAny<long>()))
                           .Returns(_initialData).Verifiable();
            var result = (OkObjectResult)_controller.GetMetricsFromAllCluster(It.IsAny<DateTimeOffset>(),
                                                                              It.IsAny<DateTimeOffset>());
            var actualResult = ((SelectByTimePeriodRamMetricsResponse)result.Value).Metrics;
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
    }
}
