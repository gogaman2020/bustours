using BusTour.Domain.Entities;
using Infrastructure.Db.Repositories;
using Infrastructure.Db.Responses;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.PromoCodes
{
    public interface IPromoCodeRepository : ICrudRepository<PromoCode>
    {
        Task<DataSourceResponse<PromoCodeGridModel>> GetGridByRequestAsync(PromoCodeDataSourceRequest request);

        Task<PromoCode> GetByFilterAsync(PromoCodeFilter filter);
    }
}