using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace WeatherForecast.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TemperatureInfoController : ControllerBase
    {
        private readonly List<TemperatureInfo> _valueHolder;
        public TemperatureInfoController(List<TemperatureInfo> valueHolder)
        {
            _valueHolder = valueHolder;
        }

        /// <summary>
        /// Чтение показаний температуры в указанный промежуток времени. В случае отсутствия параметров выдает все имеющиеся записи
        /// </summary>
        /// <param name="startDate">Начало временного интевала</param>
        /// <param name="endDate">Конец временного интервала</param>
        /// <returns>Список найденных значений и подтверждение успешности выполнения запроса</returns>
        [HttpGet("read")]
        public IActionResult Read([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            startDate = startDate.HasValue ? startDate : DateTime.MinValue;
            endDate = endDate.HasValue ? endDate : DateTime.MaxValue;

            var items = from TemperatureInfo in _valueHolder
                        where TemperatureInfo.Date >= startDate.Value && TemperatureInfo.Date <= endDate.Value
                        select TemperatureInfo;
            return Ok(items);
        }

        /// <summary>
        /// Добавляет значение температуры в указанное время
        /// </summary>
        /// <param name="temperature">Температура</param>
        /// <param name="date">Время</param>
        /// <returns>Код подтверждения обработки запроса</returns>
        [HttpPost("save")]
        public IActionResult Save([FromQuery] int? temperature, DateTime? date)
        {
            if (temperature != null && date != null)
            {
                TemperatureInfo newRecord = new() { Temperature = (int)temperature, Date = (DateTime)date };
                _valueHolder.Add(newRecord);
                return Ok();
            }
            return BadRequest();    //В случае отсутствия какого-либо параметра выдает ошибку исполнения запроса
        }

        /// <summary>
        /// Удаляет значение температуры в указанном промежутке времени
        /// </summary>
        /// <param name="startDate">Начало временного интевала</param>
        /// <param name="endDate">Конец временного интервала</param>
        /// <returns>Код подтверждения обработки запроса</returns>
        [HttpDelete("delete")]
        public IActionResult Delete([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            if (startDate == null || endDate == null)
            {
                return BadRequest();
            }

            var items = from TemperatureInfo in _valueHolder
                        where TemperatureInfo.Date >= startDate.Value && TemperatureInfo.Date <= endDate.Value
                        select TemperatureInfo;

            //ToList, для исключения exception при удалении элементов
            foreach (var item in items.ToList())
            {
                _valueHolder.Remove(item);
            }
            return Ok();
        }

        /// <summary>
        /// Обновляет значение температуры в указанное время
        /// </summary>
        /// <param name="temperature">Новое значение температуры</param>
        /// <param name="date">Время</param>
        /// <returns>Код подтверждения обработки запроса</returns>
        [HttpPut("update")]
        public IActionResult Update([FromQuery] int? temperature, DateTime? date)
        {
            if (!date.HasValue && !temperature.HasValue)
            {
                return BadRequest();
            }
            try
            {
                var updatedTemperature = _valueHolder.Single(TemperatureInfo => TemperatureInfo.Date == date.Value);
                updatedTemperature.Temperature = temperature.Value;
            }
            catch (Exception)    //В случае отсутствия значений в указанное время
            {
                return BadRequest();
            }
            return Ok();
        }
    }
}
