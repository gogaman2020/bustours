using BusTour.Data.Repositories.Languages.Queries;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Db.Common;
using Infrastructure.Db.Repositories;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.Languages
{
    [InjectAsSingleton]
    public class LanguageRepository : CrudRepository<Language, GetLanguagesQuery>, ILanguageRepository
    {
        private readonly ILogger _logger;

        public LanguageRepository()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public async Task<List<Language>> GetLanguagesAsync()
        {
            try
            {
                var languages = await _db.QueryAsync<Language>(FilterQueryObject.For(new GetLanguagesQuery(), GetLanguagesQuery.SelectByFilter));

                return languages.ToList();
            }
            catch (Exception e)
            {
                _logger.Error(e, "ReferenceRepository.GetLanguages threw error");
                throw;
            }
        }
    }
}
