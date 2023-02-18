using BusTour.AppServices.TourProcess;
using BusTour.Data.Repositories.Tours;
using BusTour.Data.Repositories.Users;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Extensions;
using BusTour.Domain.Models;
using BusTour.Domain.Models.Auth;
using Infrastructure.Common.DI;
using Infrastructure.Common.Context;
using Infrastructure.Mediator;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using BusTour.Domain.Models.Filters;

namespace BusTour.AppServices.TourService.Queries
{
    [InjectAsScoped]
    public class FilterToursQuery : MediatorQuery<List<Tour>>
    {
        private TourFilter _filter { get;  }

        public FilterToursQuery(TourFilter filter) : base()
        {
            _filter = filter;
        }

        public override async Task<MediatorCommandResult<List<Tour>>> ExecuteAsync()
        {
            _filter.DepartureDateFrom = _filter.DepartureDateFrom?.Date;
            _filter.DepartureDateTo = _filter.DepartureDateTo?.Date.AddDays(1).AddSeconds(-1);

            _filter.ArrivalDateFrom = _filter.ArrivalDateFrom?.Date;
            _filter.ArrivalDateTo = _filter.ArrivalDateTo?.Date.AddDays(1).AddSeconds(-1);

            var tours = await IoC.GetRequiredService<ITourRepository>().SelectAsync(_filter);
            return Success(tours.OrderBy(x => x.Departure).ToList());
        }
    }
}