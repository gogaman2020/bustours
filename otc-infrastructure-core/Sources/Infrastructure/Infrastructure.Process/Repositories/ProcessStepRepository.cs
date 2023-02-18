using Infrastructure.Common.Json;
using Infrastructure.Db.Common;
using Infrastructure.Db.Repositories;
using Infrastructure.Process.Repositories.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Infrastructure.Process.Repositories
{
    /// <summary>
    /// Репозитарий шагов процесса
    /// </summary>
    /// <typeparam name="T">Тип объекта состояния</typeparam>
    public class ProcessStepRepository<T> : IProcessStepRepository
        where T : ProcessData
    {
        private readonly Dictionary<string, string> _stepsDic;
        private readonly T _processData;
        private readonly Func<T, Task> _funcSave;

        public ProcessStepRepository(T process, Func<T, Task> funcSave)
        {
            _processData = process;
            _funcSave = funcSave;
            _stepsDic = process.StepsData;
        }

        public int ProcessId => _processData.Id;
        public string CurrentStepName => _processData.CurrentStepName;

        public async Task UpdateCurrentStepNameAsync(string currentStep)
        {
            _processData.CurrentStepName = currentStep;
            await SaveProcessAsync();
        }

        public Task<object> GetCurrentStepDataAsync(Type objectType)
        {
            var name = CurrentStepName;
            _stepsDic.TryGetValue(name ?? string.Empty, out var data);
            if (string.IsNullOrEmpty(data))
            {
                return Task.FromResult<object>(null);
            }

            return Task.FromResult(data.FromJson(objectType));
        }

        public async Task SaveStepDataAsync(object data)
        {
            _stepsDic[CurrentStepName] = data?.ToJson();
            await SaveProcessAsync();
        }

        private Task SaveProcessAsync()
        {
            return _funcSave(_processData);
        }
    }
}
