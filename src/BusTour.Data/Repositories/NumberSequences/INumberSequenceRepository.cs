using BusTour.Domain.Entities;
using Infrastructure.Db.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.NumberSequences
{
    public interface INumberSequenceRepository : ICrudRepository<NumberSequence>
    {
        /// <summary>
        /// Получаем следующий номер
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        Task<int> Increment(Type type, string sequence);

        async Task<string> Increment(Type type, DateTime dateTime)
        {
            var sequence = dateTime.ToString("ddMMyy");
            return sequence + "/" + (await Increment(type, sequence)).ToString();
        }
    }
}
