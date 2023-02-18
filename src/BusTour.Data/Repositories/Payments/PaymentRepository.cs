using BusTour.Data.Repositories.Payments.Queries;
using BusTour.Domain.Entities;
using BusTour.Domain.Models.Filters;
using Infrastructure.Common.DI;
using Infrastructure.Db.Common;
using Infrastructure.Db.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.Payments
{
    [InjectAsSingleton]
    public class PaymentRepository : CrudRepository<Payment, PaymentQuery>, IPaymentRepository
    {
        public async Task<List<Payment>> SelectByFilterAsync(PaymentFilter filter)
        {
            var payments = await _db.QueryAsync<Payment>(FilterQueryObject.For(filter, PaymentQuery.SelectByFilter, true));

            return payments.ToList();
        }

        public override async Task<int> SaveOrUpdateAsync(Payment entity)
        {
            var payment = (await SelectByFilterAsync(new PaymentFilter 
            { 
                OrderId = entity.OrderId,
                GiftCertificateId = entity.GiftCertificateId
            })).FirstOrDefault();

            if (payment != null)
            {
                entity.Id = payment.Id;
            }

            return await base.SaveOrUpdateAsync(entity);
        }
    }
}
