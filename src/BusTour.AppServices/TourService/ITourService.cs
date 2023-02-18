using BusTour.Data.Repositories.Tours;
using BusTour.Domain.Entities;
using BusTour.Domain.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace BusTour.AppServices.TourService
{
    public interface ITourService
    {
        Task<RouteInfo> GetRouteInfoAsync(short routeId);

        Task<MenuInfo> GetMenuInfoAsync();

        //Task<List<Tour>> SelectByFilterAsync(TourFilter filter);
    }
}
