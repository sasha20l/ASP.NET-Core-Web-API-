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
    public class HddMetricsControllerUnitTest
    {
        private readonly HddMetricsController _controller;
        private readonly Mock<IHddMetricsRepository> _mockRepository;
        private readonly Mock<ILogger<HddMetricsController>> _mockLogger;
        private readonly List<HddMetric> _initialData;
        private readonly IMapper _mapper;

        public HddMetricsControllerUnitTest()
        {
            _mockRepository = new Mock<IHddMetricsRepository>();
            _mockLogger = new Mock<ILogger<HddMetricsController>>();
            _mapper = new MapperConfiguration(mp => mp.AddProfile(new MapperProfile())).CreateMapper();
            _controller = new HddMetricsController(_mockLogger.Object, _mockRepository.Object, _mapper);
            _initialData = new Fixture().Create<List<HddMetric>>();
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
            var actualResult = ((SelectByTimePeriodHddMetricsResponse)result.Value).Metrics;
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
            var actualResult = ((SelectByTimePeriodHddMetricsResponse)result.Value).Metrics;
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
