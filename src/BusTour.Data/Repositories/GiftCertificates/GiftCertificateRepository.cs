using BusTour.Data.Repositories.GiftCertificates.Queries;
using BusTour.Data.Repositories.NumberSequences;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Db.Common;
using Infrastructure.Db.Common.Crud;
using Infrastructure.Db.Repositories;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.GiftCertificates
{
    [InjectAsSingleton]
    public class GiftCertificateRepository : CrudRepository<GiftCertificate, GiftCertificateQuery>, IGiftCertificateRepository
    {
        private readonly ILogger _logger = LogManager.GetLogger(typeof(GiftCertificateRepository).Name);
        private readonly INumberSequenceRepository _numberSequenceRepository;

        public GiftCertificateRepository()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _numberSequenceRepository = IoC.GetRequiredService<INumberSequenceRepository>();
        }

        public async Task<List<GiftCertificate>> FilterAsync(GiftCertificatesFilter filter)
        {
            var entities = await _db.QueryAsync<GiftCertificate>(FilterQueryObject.For(filter, GiftCertificateQuery.SelectByFilter, true));
           
            if (filter.Statuses.Any())
            {
                entities = entities.Where(x => filter.Statuses.Contains(x.Status));
            }

            await FillNestedAsync(entities.ToArray());
            return entities.ToList();
        }


        public async Task<List<GiftCertificateAmountVariant>> GetAmountVariantsAsync()
        {
            return (await _db.QueryAsync<GiftCertificateAmountVariant>(new CrudQueryObject<GiftCertificateAmountVariant, GiftCertificateAmountVariantQuery>(null, CrudOperation.Select, true))).ToList();
        }

        public override async Task<int> SaveOrUpdateAsync(GiftCertificate certificate)
        {
            try
            {
                if (string.IsNullOrEmpty(certificate.Number))
                {
                    certificate.Number = GenerateNumber(certificate);
                }

                await SaveClient(certificate);

                var certificateId = await base.SaveOrUpdateAsync(certificate);

                await _db.ExecuteAsync(new CrudQueryObject<GiftCertificateSurprise, GiftCertificateSurpriseQuery>(new GiftCertificateSurprise { CertificateId = certificateId }, CrudOperation.Delete));

                foreach (var certificateSurprise in certificate.CertificateSurprises)
                {
                    certificateSurprise.CertificateId = certificateId;
                    await _db.ExecuteAsync(new CrudQueryObject<GiftCertificateSurprise, GiftCertificateSurpriseQuery>(certificateSurprise, CrudOperation.Insert));
                }

                return certificateId;
            }
            catch (Exception e)
            {
                _logger.Error(e, "CrudRepository.AddTourAsync threw error");
                throw;
            }
        }

        protected string GenerateNumber(GiftCertificate certificate)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var length = 6;

            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        protected override async Task FillNestedAsync(GiftCertificate[] entities)
        {
            foreach(var certificate in entities)
            {
                certificate.CertificateSurprises = (await _db.QueryAsync<GiftCertificateSurprise>(new CrudQueryObject<GiftCertificateSurprise, GiftCertificateSurpriseQuery>(new GiftCertificateSurprise { CertificateId = certificate.Id }, CrudOperation.Select, true))).ToList();
                if (certificate.AmountVariantId.HasValue)
                {
                    certificate.AmountVariant = (await _db.QueryAsync<GiftCertificateAmountVariant>(new CrudQueryObject<GiftCertificateAmountVariant, GiftCertificateAmountVariantQuery>(new GiftCertificateAmountVariant { Id = certificate.AmountVariantId.Value }, CrudOperation.Select, true))).FirstOrDefault();
                }
                certificate.Payment = (await _db.QueryAsync<Payment>(FilterQueryObject.For(new Payment { GiftCertificateId = certificate.Id }, GiftCertificateQuery.SelectPayment))).FirstOrDefault();
                certificate.Order = (await _db.QueryAsync<Order>(FilterQueryObject.For(new Order { CertificateId = certificate.Id }, GiftCertificateQuery.SelectOrder))).FirstOrDefault();
                if (certificate.ClientId.HasValue)
                {
                    certificate.Client = (await _db.QueryAsync<Client>(FilterQueryObject.For(new Client { Id = certificate.ClientId.Value }, GiftCertificateQuery.SelectClient))).FirstOrDefault();
                }
            }
        }

        private async Task SaveClient(GiftCertificate certificate)
        {
            if (certificate.Client != null)
            {
                certificate.Client.PhoneNumber = new string(certificate.Client.PhoneNumber?.Where(c => char.IsDigit(c)).ToArray());
                certificate.Client.Id = await _db.QueryFirstOrDefaultAsync<int>(FilterQueryObject.For(certificate.Client, GiftCertificateQuery.UpsertClient));
                certificate.ClientId = certificate.Client.Id;
            }
        }
    }
}
