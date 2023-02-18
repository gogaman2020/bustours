using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Enums;
using BusTour.Domain.Models;
using Infrastructure.Common.DI;
using NLog;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.AppServices.TourService
{
    [InjectAsSingleton]
    public class TourService : ITourService
    {
        private readonly ILogger _logger;
        private readonly ITourRepository _tourRepository;

        public TourService(ITourRepository tourRepository)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _tourRepository = tourRepository;
        }

        public async Task<RouteInfo> GetRouteInfoAsync(short routeId)
        {
            var routeInfo = new RouteInfo();

            var types = new List<TourType>{ TourType.Regular, TourType.PrivateHire };

            routeInfo.Route = await _tourRepository.GetRouteAsync(routeId);
            routeInfo.Tours = (await _tourRepository.GetToursAsync(routeId)).Where(x => x.Type.HasValue && types.Contains(x.Type.Value)).ToList();

            return routeInfo;
        }

        public async Task<MenuInfo> GetMenuInfoAsync()
        {
            var menuInfo = new MenuInfo();

            menuInfo.Menus = await _tourRepository.GetMenusAsync();
            menuInfo.Beverages = await _tourRepository.GetBeveragesAsync();
            menuInfo.Allergies = await _tourRepository.GetAllergiesAsync();
            menuInfo.Surprises = await _tourRepository.GetSurprisesAsync();

            return menuInfo;
        }
    }
}
