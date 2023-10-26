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
    public class DotNetMetricsControllerUnitTest
    {
        private readonly DotNetMetricsController _controller;
        private readonly Mock<IDotNetMetricsRepository> _mockRepository;
        private readonly Mock<ILogger<DotNetMetricsController>> _mockLogger;
        private readonly List<DotNetMetric> _initialData;
        private readonly IMapper _mapper; 

        public DotNetMetricsControllerUnitTest()
        {
            _mockRepository = new Mock<IDotNetMetricsRepository>();
            _mockLogger = new Mock<ILogger<DotNetMetricsController>>();
            _mapper = new MapperConfiguration(mp => mp.AddProfile(new MapperProfile())).CreateMapper();
            _controller = new DotNetMetricsController(_mockRepository.Object, _mockLogger.Object, _mapper);
            _initialData = new Fixture().Create<List<DotNetMetric>>();
        }

        [Fact]
        public void GetAll_ShouldCall_GetAll_From_Repository()
        {
            _mockRepository.Setup(repository => repository.GetAll()).Returns(_initialData).Verifiable();
            var result = (OkObjectResult)_controller.GetAll();
            var actualResult = ((AllDotNetMetricsResponse)result.Value).Metrics;
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
            var result = (OkObjectResult)_controller.GetDotNetMetrics(It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>());
            var actualResult = ((SelectByTimePeriodDotNetMetricsResponse)result.Value).Metrics;
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
