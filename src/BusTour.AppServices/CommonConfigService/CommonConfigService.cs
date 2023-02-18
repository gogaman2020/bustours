using BusTour.Data.Repositories.CommonConfigs;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using NLog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.AppServices.CommonConfigService
{
    [InjectAsSingleton]
    public class CommonConfigService : ICommonConfigService
    {
        private readonly ILogger _logger;
        private readonly ICommonConfigRepository _commonConfigRepository;

        public CommonConfigService(ICommonConfigRepository commonConfigRepository)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _commonConfigRepository = commonConfigRepository;
        }
        public async Task<List<CommonConfig>> GetCommonConfigAsync()
        {
            var config = await _commonConfigRepository.GetCommonConfigAsync();

            return config;
        }
    }
}
