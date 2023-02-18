using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Models.Filters;
using Infrastructure.Db.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.Tours
{
    public interface ITourRepository: ICrudRepository<Tour>
    {
        Task<List<Tour>> SelectAsync(TourFilter filter);

        Task<List<Tour>> SelectByIdAsync(TourFilter filter);

        Task<TourGridModel> SelectTourFB(TourFilter filter);

        Task<List<CurrentBookingGridModel>> SelectCurrentBookings(TourFilter filter);

        Task<Route> GetRouteAsync(short id);

        Task<List<Route>> GetRoutesAsync();

        Task<List<Bus>> GetBusesAsync();

        Task<Tour> GetTourAsync(int id);

        Task<List<Tour>> GetToursAsync(short routeId);

        Task<List<Menu>> GetMenusAsync();

        Task<List<Beverage>> GetBeveragesAsync();

        Task<List<Allergy>> GetAllergiesAsync();

        Task<List<Surprise>> GetSurprisesAsync();

        Task<List<City>> GetCitiesAsync();

        Task UpdateServiceTourAsync(Tour tour);
    }
}
