using BusTour.Domain.Entities;
using BusTour.Domain.Enums;
using Infrastructure.Db.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.GiftCertificates
{
    public interface IGiftCertificateRepository: ICrudRepository<GiftCertificate>
    {
        /// <summary>
        /// Получить варианты цены
        /// </summary>
        /// <returns></returns>
        Task<List<GiftCertificateAmountVariant>> GetAmountVariantsAsync();

        /// <summary>
        /// Получить сертификаты по фильтру
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        Task<List<GiftCertificate>> FilterAsync(GiftCertificatesFilter filter);
    }

    public class GiftCertificatesFilter
    {
        public List<GiftCertificateStatus> Statuses { get; set; }
        public string Number { get; set; }

        public GiftCertificatesFilter()
        {
            Statuses = new List<GiftCertificateStatus>();
        }
    }
}
