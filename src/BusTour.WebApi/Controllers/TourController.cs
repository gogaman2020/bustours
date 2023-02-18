using BusTour.AppServices.TourService;
using BusTour.AppServices.TourService.Commands;
using BusTour.AppServices.TourService.Queries;
using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using BusTour.Domain.Models;
using BusTour.Domain.Models.Filters;
using BusTour.Domain.Models.Tour;
using BusTour.AppServices.BookingService.Queries;
using Infrastructure.Common.DI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BusTour.Domain.Models.Responses;

namespace BusTour.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [InjectAsSingleton]
    public class TourController : BusTourControllerBase
    {
        private readonly ITourService _tourService;
        private readonly ITourRepository _tourRepository;
        public TourController(IActionContextAccessor actionContextAccessor, ITourService tourService)
        {
            _tourService = tourService;
            _tourRepository = IoC.GetRequiredService<ITourRepository>();
        }

        [HttpGet]
        [Route("GetRouteInfo/{routeId}")]
        public async Task<RouteInfo> GetRouteInfo(short routeId)
        {
            try
            {
                var routeInfo = await _tourService.GetRouteInfoAsync(routeId);

                return routeInfo;
            }
            catch (Exception e)
            {
                _logger.Error(e, $"{String.Join('/', _actionContextAccessor.ActionContext.RouteData.Values.Select(i => i.Value))} threw exception");
                throw;
            }
        }

        [HttpGet]
        [Route("GetMenuInfo")]
        public async Task<MenuInfo> GetMenuInfo()
        {
            try
            {
                var menuInfo = await _tourService.GetMenuInfoAsync();

                return menuInfo;
            }
            catch (Exception e)
            {
                _logger.Error(e, $"{String.Join('/', _actionContextAccessor.ActionContext.RouteData.Values.Select(i => i.Value))} threw exception");
                throw;
            }
        }

        [HttpGet]
        [Route("get-menu-beverage/{tourId}")]
        public async Task<Tour>  GetMenuBeverage(int tourId)
        {
            try
            {
                var ids = new TourFilter { Ids = new List<int>() { tourId } };
                return (await _tourRepository.SelectAsync(ids)).FirstOrDefault();

            }
            catch (Exception e)
            {
                _logger.Error(e, $"{String.Join('/', _actionContextAccessor.ActionContext.RouteData.Values.Select(i => i.Value))} threw exception");
                throw;
            }
        }

        [HttpGet]
        [Route("get-tour/{tourId}")]
        public async Task<Tour> GetTourById(int tourId)
        {
            try
            {
                var ids = new TourFilter { Ids = new List<int>() { tourId } };
                var tour = (await _tourRepository.SelectByIdAsync(ids)).FirstOrDefault();
                return tour;

            }
            catch (Exception e)
            {
                _logger.Error(e, $"{String.Join('/', _actionContextAccessor.ActionContext.RouteData.Values.Select(i => i.Value))} threw exception");
                throw;
            }
        }

        [HttpGet]
        [Route("get-tour-fb")]
        public async Task<TourGridModel> GetTourFB([FromQuery] string filter)
        {
            try
            {
                return await _tourRepository.SelectTourFB(JsonConvert.DeserializeObject<TourFilter>(filter));
            }
            catch (Exception e)
            {
                _logger.Error(e, $"{String.Join('/', _actionContextAccessor.ActionContext.RouteData.Values.Select(i => i.Value))} threw exception");
                throw;
            }
        }

        [HttpGet]
        [Route("GetCurrentBookings")]
        public async Task<List<CurrentBookingGridModel>> GetCurrentBookings([FromQuery] string filter)
        {
            try
            {
                var tours = await _tourRepository.SelectCurrentBookings(JsonConvert.DeserializeObject<TourFilter>(filter));
                foreach (var item in tours)
                {
                    var result = await RunCommandAsync(new CheckOrdersConflictsQuery(new OrderFilter
                    {
                        TourIds = new List<int> { item.Id }
                    }));
                    item.Conflicts = (result.Result as dynamic).Value.Count>0;
                }
                return tours;

            }
            catch (Exception e)
            {
                _logger.Error(e, $"{String.Join('/', _actionContextAccessor.ActionContext.RouteData.Values.Select(i => i.Value))} threw exception");
                throw;
            }
        }

        [HttpPost]
        [Route("create-tours")]
        public async Task<ActionResult<List<Tour>>> CreateTours(CreateToursCommand command)
        {
            return await RunCommandAsync(command);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Tour>> GetTour(int id)
        {
            return await RunCommandAsync(new GetTourQuery { Id = id.ToString() });
		}

        [HttpGet]
        [Route("Filter")]
        public async Task<ActionResult<List<Tour>>> Filter([FromQuery] string filter)
        {
            return await RunCommandAsync(new FilterToursQuery(JsonConvert.DeserializeObject<TourFilter>(filter)));
        }

        [HttpPost]
        [Route("Delete")]
        public async Task<ActionResult<List<int>>> Delete(List<int> ids)
        {
            return await RunCommandAsync(new DeleteToursCommand(ids));
            //return ids;
        }

        [HttpGet]
        [Route("GetCities")]
        public async Task<ActionResult<List<City>>> GetCities()
        {
            return await RunCommandAsync(new GetCitiesQuery());
        }

        [HttpGet]
        [Route("GetRoutes")]
        public async Task<ActionResult<List<Route>>> GetRoutes()
        {
            return await RunCommandAsync(new GetRoutesQuery());
        }

        [HttpGet]
        [Route("GetBuses")]
        public async Task<ActionResult<List<Bus>>> GetBuses()
        {
            return await RunCommandAsync(new GetBusesQuery());
		}
            
        [HttpGet("GetTourInformation/{tourId}")]
        public async Task<ActionResult<TourInformationModel>> GetTourInformation(int tourId)
        {
            return await RunCommandAsync(new GetTourInformationQuery { Id = tourId.ToString() });
        }

        [HttpPost]
        [Route("Cancel")]
        public async Task<ActionResult<bool>> Cancel(CancelTourCommand command)
        {
            return await RunCommandAsync(command);
        }

        [HttpPut("UpdateServiceTour")]
        public async Task<ActionResult<BaseResponse>> UpdateServiceTour(Tour tour)
        {
            return await RunCommandAsync(new UpdateServiceTourCommand(tour));
        }

        [HttpPost]
        [Route("DeleteTour")]
        public async Task<ActionResult<BaseResponse>> DeleteTour(DeleteTourCommand command)
        {
            return await RunCommandAsync(command);
        }
    }
}
