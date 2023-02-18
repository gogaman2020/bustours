using BusTour.Domain.Entities;
using Infrastructure.Db.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.Languages
{
    public interface ILanguageRepository : ICrudRepository<Language>
    {
        Task<List<Language>> GetLanguagesAsync();
    }
}
