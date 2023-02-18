using BusTour.AppServices.SelectionService;
using BusTour.Domain.Entities;
using BusTour.Domain.Models.Responses;
using Infrastructure.Common.DI;
using Microsoft.AspNetCore.Mvc;
using Infrastructure.Common.Json;
using NLog;
using System;
using System.Threading.Tasks;
using BusTour.Domain.Models.Selection;

namespace BusTour.WebApi.Controllers
{
    /// <summary>
    /// Контроллер для подбора мест.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [InjectAsSingleton]
    public class SelectionController : BusTourControllerBase
    {
        private readonly ILogger _logger;
        private readonly ISelectionService _selectionService;

        /// <summary>
        /// Конструктор класса <see cref="SelectionController"/>
        /// </summary>
        /// <param name="selectionService">Сервис подбора мест.</param>
        public SelectionController(ISelectionService selectionService)
        {
            _logger = LogManager.GetCurrentClassLogger();

            _selectionService = selectionService;
        }

        /// <summary>
        /// Получить информацию о состоянии автобуса.
        /// </summary>
        /// <param name="request">Информация о выборе пользователя.</param>
        /// <returns>Ответ на запрос сведений о статусах мест.</returns>
        [HttpPost]
        [Route("get-bus-model")]
        public Task<ResponseSelection> GetBusModel(SelectionInfo request)
        {
            return _selectionService.SelectAsync(request);
        }

        [HttpGet("GetSelectableBusModelForOrder")]
        public async Task<ResponseSelection> GetSelectableBusModelForOrder(string info)
        {
            return await _selectionService.GetSelectableBusModelForOrderAsync(info.FromJson<SelectionInfo>());
        }
    }
}
