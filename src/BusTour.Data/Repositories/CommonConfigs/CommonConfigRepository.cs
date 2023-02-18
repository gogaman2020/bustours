using BusTour.Data.Repositories.CommonConfigs.Queries;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Db.Common;
using Infrastructure.Db.Repositories;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.CommonConfigs
{
    [InjectAsSingleton]
    public class CommonConfigRepository : CrudRepository<CommonConfig, GetCommonConfigQuery>, ICommonConfigRepository
    {
        private readonly ILogger _logger;

        public CommonConfigRepository()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<List<CommonConfig>> GetCommonConfigAsync()
        {
            try
            {
                var config = await _db.QueryAsync<CommonConfig>(FilterQueryObject.For(new GetCommonConfigQuery(), GetCommonConfigQuery.SelectByFilter));

                return config.ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e, "CommonConfigRepository.GetCommonConfigAsync threw error");
                throw;
            }
        }
    }
}
