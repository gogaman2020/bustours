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

namespace BusTour.AppServices.TourService.Commands
{
    [InjectAsScoped]
    public class DeleteToursCommand : HighLevelMediatorCommand<List<int>>
    {
        private readonly List<int> _ids;

        private readonly ITourRepository _tourRepository;


        public DeleteToursCommand(List<int> ids)
        {
            _ids = ids;
            _tourRepository = IoC.GetRequiredService<ITourRepository>();
        }

        public override async Task<MediatorCommandResult<List<int>>> ExecuteAsync()
        {
            var tours = await _tourRepository.GetAsync(_ids);

            await _tourRepository.DeleteAsync(tours);

            return Success(tours.Select(x => x.Id).ToList());
        }
    }
}