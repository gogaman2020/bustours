using BusTour.Data.Repositories.Languages;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using NLog;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTour.AppServices.ReferenceService
{
    [InjectAsSingleton]
    public class ReferenceService : IReferenceService
    {
        private readonly ILogger _logger;
        private readonly ILanguageRepository _referenceRepository;

        public ReferenceService(ILanguageRepository referenceRepository)
        {
            _logger = LogManager.GetCurrentClassLogger();
            _referenceRepository = referenceRepository;
        }

        public async Task<List<Language>> GetLanguagesAsync()
        {
            var languages = await _referenceRepository.GetLanguagesAsync();

            return languages;
        }
    }
}
