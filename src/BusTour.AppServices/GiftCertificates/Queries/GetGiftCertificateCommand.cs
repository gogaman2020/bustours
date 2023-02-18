using BusTour.Data.Repositories.GiftCertificates;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Mediator;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.AppServices.GiftCertificates.Queries
{
    public class GetGiftCertificateCommand : HighLevelMediatorCommand<GiftCertificate>
    {
        public override async Task<MediatorCommandResult<GiftCertificate>> ExecuteAsync()
        {
            if (!int.TryParse(Id, out int id))
            {
                return Fail("Type mismatch");
            }

            return Success(await IoC.GetRequiredService<IGiftCertificateRepository>().GetAsync(id));
        }
    }
}
