using AutoFixture;
using AutoMapper;
using MetricsAgent;
using MetricsAgent.Controllers;
using MetricsAgent.DTO;
using MetricsAgent.Models;
using MetricsAgent.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace MetricsAgentTests
{
    public class NetworkMetricsControllerUnitTest
    {
        private readonly NetworkMetricsController _controller;
        private readonly Mock<INetworkMetricsRepository> _mockRepository;
        private readonly Mock<ILogger<NetworkMetricsController>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly List<NetworkMetric> _initialData;

        public NetworkMetricsControllerUnitTest()
        {
            _mockRepository = new Mock<INetworkMetricsRepository>();
            _mockLogger = new Mock<ILogger<NetworkMetricsController>>();
            _mapper = new MapperConfiguration(mp => mp.AddProfile(new MapperProfile())).CreateMapper();
            _controller = new NetworkMetricsController(_mockRepository.Object, _mockLogger.Object, _mapper);
            _initialData = new Fixture().Create<List<NetworkMetric>>();
        }

        [Fact]
        public void GetAll_ShouldCall_GetAll_From_Repository()
        {
            _mockRepository.Setup(repository => repository.GetAll()).Returns(_initialData).Verifiable();
            var result = (OkObjectResult)_controller.GetAll();
            var actualResult = ((AllNetworkMetricsResponse)result.Value).Metrics;
            _mockRepository.Verify(repository => repository.GetAll(), Times.AtLeastOnce());
            for (int i = 0; i < _initialData.Count; i++)
            {
                Assert.Equal(_initialData[i].Value, actualResult[i].Value);
                Assert.Equal(_initialData[i].Id, actualResult[i].Id);
                Assert.Equal(_initialData[i].Time, actualResult[i].Time.ToUnixTimeSeconds());
            }
        }
        [Fact]
        public void GetByTimePeriod_ShouldCall_GetByTimePeriod_From_Repository()
        {
            _mockRepository.Setup(repository => repository.GetByTimePeriod(It.IsAny<long>(), It.IsAny<long>()))
                                   .Returns(_initialData).Verifiable();
            var result = (OkObjectResult)_controller.GetNetworkMetrics(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>());
            var actualResult = ((SelectByTimePeriodNetworkMetricsResponse)result.Value).Metrics;
            _mockRepository.Verify(repository => repository.GetByTimePeriod(It.IsAny<long>(), It.IsAny<long>()),
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
