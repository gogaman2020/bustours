using BusTour.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BusTour.AppServices.ReferenceService
{
    public interface IReferenceService
    {
        Task<List<Language>> GetLanguagesAsync();
    }
}
