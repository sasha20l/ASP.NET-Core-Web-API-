using MetricsManager.DAL;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace MetricsManager.Controllers
{
    [Route("api/agents")]
    [ApiController]
    public class AgentsController : ControllerBase
    {
        private readonly IAgentsRepository _agentsRepository;
        private readonly ILogger<AgentsController> _logger;        
        public AgentsController(IAgentsRepository agentsRepository, ILogger<AgentsController> logger)
        {
            _agentsRepository = agentsRepository;
            _logger = logger;
            _logger.LogDebug(1, "AgentController created");
        }

        /// <summary>
        /// Регистрирует нового агента
        /// </summary>
        /// <param name="agentInfo">Принимает в качестве параметра объект AgentInfo из тела сообщения</param>
        /// <returns>В случае успешной обработки запроса возвращает код "200"</returns>
        [HttpPost("register")]
        public IActionResult RegisterAgent([FromBody] AgentInfo agentInfo)
        {
            _logger.LogTrace($"Agent registered with params: AgentID={agentInfo.AgentId}, AgentAdress={agentInfo.AgentUrl}");
            if (!IsAgentRegistered(agentInfo))
            {
                _agentsRepository.Create(agentInfo);
                return Ok();
            }
            else
            {
                return BadRequest("Агент с таким URL уже зарегистрирован!");
            }
        }
        private bool IsAgentRegistered(AgentInfo agent)
        {
            return _agentsRepository.FindUrl(agent.AgentUrl) != null;
        }

        /// <summary>
        /// Удаляет агента по указанному ID
        /// </summary>
        /// <param name="agentId">ID агента</param>
        /// <returns>В случае успешной обработки запроса возвращает код "200"</returns>
        [HttpDelete("unregister/{agentId}")]
        public IActionResult DeleteAgent([FromRoute] int agentId)
        {
            _logger.LogTrace($"Agent unregistered: AgentID={agentId}");
            _agentsRepository.Delete(agentId);
            return Ok();
        }
        /// <summary>
        /// Возвращает список всех зарегистрированных агентов
        /// </summary>
        /// <returns>Список агентов в формате AgentInfo</returns>
        [HttpGet("getAll")]
        public IActionResult GetAllAgents()
        {
            _logger.LogTrace($"Query for all registered agents");
            var response = _agentsRepository.GetAll();
            return Ok(response);
        }
        /// <summary>
        /// Возвращает информацию об агенте по его Id
        /// </summary>
        /// /// <param name="agentId">ID агента</param>
        /// <returns>Объект агента в формате AgentInfo</returns>
        [HttpGet("getById/{agentId}")]
        public IActionResult GetAgentById([FromRoute]int agentId)
        {
            _logger.LogTrace($"Query for agent info for Agent: {agentId}");
            var response = _agentsRepository.GetAgentById(agentId);
            return Ok(response);
        }
    }
}

