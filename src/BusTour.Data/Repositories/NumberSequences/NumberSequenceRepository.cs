using BusTour.Data.Repositories.Notifications.Queries;
using BusTour.Data.Repositories.NumberSequences.Queries;
using BusTour.Domain.Entities;
using Infrastructure.Common.DI;
using Infrastructure.Db.Common;
using Infrastructure.Db.Repositories;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.Data.Repositories.NumberSequences
{
    [InjectAsSingleton]
    public class NumberSequenceRepository : CrudRepository<NumberSequence, NumberSequenceQuery>, INumberSequenceRepository
    {
        public async Task<int> Increment(Type type, string sequence)
        {
            return await _db.QueryFirstOrDefaultAsync<int>(FilterQueryObject.For(new NumberSequence
            {
                Sequence = type.Name + "/" + sequence
            }, NumberSequenceQuery.Increment));
        }
    }
}
