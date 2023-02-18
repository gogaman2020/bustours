using BusTour.AppServices.SelectionService;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Models.Bus;
using BusTour.Domain.Models.Responses;
using BusTour.Domain.Models.Selection;
using Infrastructure.Common.DI;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NLog;
using System.Linq;

namespace BusTour.WebApi.Controllers
{
    /// <summary>
    /// Контроллер для подбора мест.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    [InjectAsSingleton]
    public class TestSelectionController : ControllerBase
    {
        private readonly ILogger _logger;

        private readonly ITestSelectionService _selectionService;

        const string sessionKey = "BusModel";

        /// <summary>
        /// Конструктор класса <see cref="TestSelectionController"/>
        /// </summary>
        /// <param name="selectionService">Сервис подбора мест.</param>
        public TestSelectionController(ITestSelectionService selectionService)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _selectionService = selectionService;
        }

        [HttpPost]
        [Route("get-bus-model")]
        public TestResponseBus GetBusModel(SelectionInfo selectionInfo)
        {
            return GetResponse(selectionInfo, false);
        }

        [HttpPost]
        [Route("lock-switch")]
        public TestResponseBus LockSwitch(SelectionInfo selectionInfo)
        {
            return GetResponse(selectionInfo, true);
        }

        [HttpPost]
        [Route("get-rules-debug-info")]
        public TestResponseDebugInfo GetRulesDebugInfo(SelectionInfo selectionInfo)
        {
            var busModel = LoadBusModel();
            if (busModel == null)
                return null;

            var result = _selectionService.GetRulesDebugInfo(busModel, selectionInfo);

            return result;
        }

        private TestResponseBus GetResponse(SelectionInfo selectionInfo, bool isLockMode)
        {
            var busModel = LoadBusModel();
            if (busModel == null)
                return null;

            if (isLockMode)
                busModel.UnSelectAll();

            var selectionResult = _selectionService.Select(busModel, selectionInfo, isLockMode);
            
            SaveBusModel(busModel);

            var result = ConvertToResponse(busModel, selectionResult, selectionInfo);

            return result;
        }

        private TestBusModel LoadBusModel()
        {
            var value = HttpContext.Session.GetString(sessionKey);

            var busModel = 
                string.IsNullOrEmpty(value) 
                    ? null 
                    : JsonConvert.DeserializeObject<TestBusModel>(value);

            if (busModel == null)
            {
                busModel = TestBusModel.Parse();
                SaveBusModel(busModel);
            }

            busModel.UnSelectAll();

            return busModel;
        }

        private void SaveBusModel(TestBusModel model)
        {
            var serialized = JsonConvert.SerializeObject(model);
            HttpContext.Session.SetString(sessionKey, serialized);
        }

        private TestResponseBus ConvertToResponse(TestBusModel model, TestSelectionResult selectionResult, SelectionInfo selectionInfo)
        {
            var response = 
                new TestResponseBus 
                {
                    SelectionResult = new TestResponseSelectionResult
                        { 
                            IsAutoSelect = selectionResult.IsAutoSelect,
                            Path = selectionResult.Path
                        },
                    Selection = selectionInfo,
                    Tables = 
                        model.Tables
                            .ToDictionary(
                                p => p.Id.ToString(),
                                p => new TestResponseTable
                                    {
                                        Id = p.Id,
                                        Number = p.Number,
                                        Locked = p.IsLocked,
                                        Selected = p.IsSelected,
                                        IsRecommended = model.Recommended.Any(x => x.Type == BusObjectTypes.Table && p.Id == x.Id),
                                        Available = 
                                            selectionResult.AvailableObjects
                                                ?.Any(x => x.Type == BusObjectTypes.Table && p.Id == x.Id) == true,
                                        Seats = 
                                            p.Seats
                                                .ToDictionary(
                                                    s => s.Number,
                                                    s => new TestResponseSeat
                                                        {
                                                            Id = s.Id,
                                                            Number = s.Number,
                                                            Locked = s.IsLocked,
                                                            Selected = s.IsSelected,
                                                            IsRecommended = model.Recommended.Any(x => x.Type == BusObjectTypes.Seat && s.Id == x.Id),
                                                            Available = 
                                                                selectionResult.AvailableObjects
                                                                    ?.Any(x => x.Type == BusObjectTypes.Seat && s.Id == x.Id) == true
                                                        })
                                    })
                };

            return response;
        }
    }
}
