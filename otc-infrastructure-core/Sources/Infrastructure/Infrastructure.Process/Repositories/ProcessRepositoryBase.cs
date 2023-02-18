using Infrastructure.Common.Exceptions;
using Infrastructure.Db.Common;
using Infrastructure.Db.Repositories;
using Infrastructure.Process.Repositories.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Infrastructure.Process.Repositories
{
    public abstract class ProcessRepositoryBase<TEntity, TState> : CrudRepository<TState, UpsertProcessStateQuery<TState>>, IProcessRepository<TEntity>
        where TState: ProcessData, IEntity, new()
    {
        public async ValueTask<IProcessStepRepository> UseProcessAsync(TEntity entity)
        {
            var state = await SelectInternalAsync(entity);
            return new ProcessStepRepository<TState>(state, SaveInternalAsync);
        }

        protected abstract int GetIdentifier(TEntity state);

        private async Task SaveInternalAsync(TState state)
        {
            if (state != null)
            {
                var filter = new FilterObject<TState>(state, UpsertProcessStateQuery<TState>.UpsertCommand);
                var result = await _db.QuerySingleOrDefaultAsync<int>(filter);
                if (result != default(int))
                {
                    state.Id = result;
                }
            }
        }

        private async Task<TState> SelectInternalAsync(TEntity state)
        {
            if (state != null)
            {
                var id = GetIdentifier(state);
                var filter = new FilterObject<TState>(new TState { ObjectId = id }, UpsertProcessStateQuery<TState>.SelectCommand);
                var result = await _db.QuerySingleOrDefaultAsync<TState>(filter);
                if (result == null)
                {
                    result = new TState
                    {
                        ObjectId = id
                    };
                }

                return result;
            }

            return null;
        }
    }
}
