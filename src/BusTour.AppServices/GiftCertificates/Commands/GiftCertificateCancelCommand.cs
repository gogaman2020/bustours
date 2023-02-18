using BusTour.AppServices.TourProcess;
using BusTour.Data.Repositories.GiftCertificates;
using BusTour.Data.Repositories.Tours;
using BusTour.Data.Repositories.Users;
using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using BusTour.Domain.Extensions;
using BusTour.Domain.Models;
using BusTour.Domain.Models.Auth;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BusTour.AppServices.GiftCertificates.Commands
{
    [InjectAsScoped]
    public class GiftCertificateCancelCommand : HighLevelMediatorCommand<GiftCertificate>
    {
        private readonly IGiftCertificateRepository _giftCertificateRepository;


        public GiftCertificateCancelCommand()
        {
            _giftCertificateRepository = IoC.GetRequiredService<IGiftCertificateRepository>();
        }

        public override async Task<MediatorCommandResult<GiftCertificate>> ExecuteAsync()
        {
            if (!int.TryParse(Id, out int id))
            {
                return Fail("Type mismatch");
            }

            var certificate = await _giftCertificateRepository.GetAsync(id);

            if (certificate != null)
            {

                certificate.Cancelled = true;

                await _giftCertificateRepository.SaveOrUpdateAsync(certificate);

                certificate = await _giftCertificateRepository.GetAsync(id);

                return Success(certificate);
            }
            else
            {
                return Fail("Not found");
            }
        }
    }
}