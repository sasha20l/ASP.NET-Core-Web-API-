using AutoFixture;
using MetricsManager;
using MetricsManager.Controllers;
using MetricsManager.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace MetricsManagerTests
{
    public class AgentsControllerUnitTest
    {
        private readonly AgentsController _controller;
        private readonly Mock<ILogger<AgentsController>> _mockLogger;
        private readonly Mock<IAgentsRepository> _mockRepository;
        private readonly List<AgentInfo> _initialData;

        public AgentsControllerUnitTest()
        {
            _mockLogger = new Mock<ILogger<AgentsController>>();
            _mockRepository = new Mock<IAgentsRepository>();
            _initialData = new Fixture().Create<List<AgentInfo>>();
            _controller = new AgentsController(_mockRepository.Object, _mockLogger.Object);
        }

        [Fact]
        public void RegisterAgent_ShouldCall_Create_From_Repository()
        {
            _mockRepository.Setup(repository => repository.Create(It.IsAny<AgentInfo>())).Verifiable();
            var result = _controller.RegisterAgent(_initialData[0]);
            _mockRepository.Verify(repository => repository.Create(It.IsAny<AgentInfo>()), Times.AtLeastOnce());
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void DeleteAgent_ShouldCall_DeleteAgent_From_Repository()
        {
            _mockRepository.Setup(repository => repository.Delete(It.IsAny<int>())).Verifiable();
            var result = _controller.DeleteAgent(_initialData[0].AgentId);
            _mockRepository.Verify(repository => repository.Delete(It.IsAny<int>()), Times.AtLeastOnce());
            _ = Assert.IsAssignableFrom<IActionResult>(result);
        }

        [Fact]
        public void GetAllAgents_ShouldCall_GetAll_From_Repository()
        {
            _mockRepository.Setup(repository => repository.GetAll()).Returns(_initialData).Verifiable();
            var result = (OkObjectResult)_controller.GetAllAgents();
            var actualResult = (List<AgentInfo>)result.Value;
            _mockRepository.Verify(repository => repository.GetAll(), Times.AtLeastOnce());
            for (int i = 0; i < _initialData.Count; i++)
            {
                Assert.Equal(_initialData[i].AgentId, actualResult[i].AgentId);
                Assert.Equal(_initialData[i].AgentUrl, actualResult[i].AgentUrl);
            }
        }
        [Fact]
        public void GetAgentById_ShouldCall_GetAgentById_From_Repository()
        {
            _mockRepository.Setup(repository => repository.GetAgentById(It.IsAny<int>()))
                                                          .Returns(_initialData[0]).Verifiable();
            var result = (OkObjectResult)_controller.GetAgentById(It.IsAny<int>());
            var actualResult = (AgentInfo)result.Value;
            _mockRepository.Verify(repository => repository.GetAgentById(It.IsAny<int>()), Times.AtLeastOnce());
            Assert.Equal(_initialData[0].AgentId, actualResult.AgentId);
            Assert.Equal(_initialData[0].AgentUrl, actualResult.AgentUrl);
        }
    }
}

