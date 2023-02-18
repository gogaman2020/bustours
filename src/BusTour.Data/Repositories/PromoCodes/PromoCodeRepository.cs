using BusTour.Data.Repositories.PromoCodes.Queries;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Db.Common;
using Infrastructure.Db.Repositories;
using Infrastructure.Db.Responses;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.PromoCodes
{
    [InjectAsSingleton]
    public class PromoCodeRepository : CrudRepository<PromoCode, PromoCodeQuery>, IPromoCodeRepository
    {
        private readonly ILogger _logger = LogManager.GetLogger(typeof(PromoCodeRepository).Name);

        public PromoCodeRepository()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public override async Task<int> SaveOrUpdateAsync(PromoCode promoCode)
        {
            try
            {
                promoCode.NumberOfUses = promoCode.NumberOfUses ?? 0;
                return await base.SaveOrUpdateAsync(promoCode);
            }
            catch (Exception e)
            {
                _logger.Error(e, "CrudRepository.AddPromoCode threw error");
                throw;
            }
        }

        public async Task<DataSourceResponse<PromoCodeGridModel>> GetGridByRequestAsync(PromoCodeDataSourceRequest request)
        {
            try
            {
                var filter = request.GetFilter();

                if (string.IsNullOrWhiteSpace(filter.City))
                {
                    filter.City = null;
                }

                //filter.IsActive = !filter.ApplyStatusFilter.HasValue || filter.IsActive == false ? null : filter.IsActive;
                //filter.IsExpiredOrAmended = !filter.ApplyStatusFilter.HasValue || filter.IsExpiredOrAmended == false ? null : filter.IsExpiredOrAmended;

                var items = await _db.QueryAsync<PromoCodeGridModel>(FilterQueryObject.For(request, PromoCodeQuery.SelectGridByFilter));
                //var count = await _db.QueryFirstOrDefaultAsync<int>(FilterQueryObject.For(request, PromoCodeQuery.SelectCount));
                //if(filter.IsActive == true) 
                //{
                //    items = items.Where(x => x.ExpirationDate >= DateTime.UtcNow || x.ExpirationDate == null);
                //}

                return new DataSourceResponse<PromoCodeGridModel>
                {
                    Items = items.ToArray(),
                    Count = items.Count(),
                };
            }
            catch (Exception e)
            {
                _logger.Error(e, "CrudRepository.GetGridByFilterAsync threw error");
                throw;
            }
        }

        public async Task<PromoCode> GetByFilterAsync(PromoCodeFilter filter)
        {
            var promoCode = await _db.QueryFirstOrDefaultAsync<PromoCode>(FilterQueryObject.For(filter, PromoCodeQuery.SelectByFilter));

            return promoCode;
        }
    }
}
