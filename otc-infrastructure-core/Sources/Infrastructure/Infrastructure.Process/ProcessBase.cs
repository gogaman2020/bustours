using Infrastructure.Common.Exceptions;
using Infrastructure.Process.Args;
using Infrastructure.Process.Commands;
using Infrastructure.Process.Helpers;
using Infrastructure.Process.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Process
{
    /// <summary>
    /// Базовый класс процесса
    /// </summary>
    /// <typeparam name="T">Тип используемого объекта</typeparam>
    /// <typeparam name="TS">Тип статуса процесса</typeparam>
    public abstract class ProcessBase<T, TS> : IProcess<T>
        where T : class
        where TS : ProcessData
    {
        private readonly IProcessRepository<T> _processRepository;
        private readonly IProcessAudit _audit;
        private IProcessStepRepository _stepRepository;
        private IProcessStep<T> _currentStep;
        private IEnumerable<Type> _steps;

        protected ProcessBase(IProcessRepository<T> repository, IProcessAudit audit)
        {
            _processRepository = repository;
            _audit = audit;
        }

        private IProcessStepRepository StepRepository
        {
            get
            {
                if (_stepRepository == null)
                {
                    throw new InvalidOperationException($"Процесс не инициализирован");
                }

                return _stepRepository;
            }
        }

        protected IEnumerable<Type> Steps
        {
            get
            {
                if (_steps == null)
                {
                    _steps = this.GetStepTypes();
                }

                return _steps;
            }
        }

        protected virtual int? ObjectId { get; }

        #region IProcess

        public async ValueTask InitStateAsync()
        {
            await UpdateCurrentStepAsync(StartStepName);
            var step = await GetCurrentStepAsync();
            var enterCommand = new StepCommandArgsEnter(null, null);
            await step.CommandAsync(enterCommand);
            await SaveCurrentStepAsync();
        }

        public void Reset()
        {
            _stepRepository = null;
            _currentStep = null;
            Context = default(T);
        }

        public async Task SendCommandAsync(int id, string name)
        {
            Reset();
            await SetContextAsync(id);
            await SendCommandAsync(new StepCommandArgs(name));
        }

        public Task SendCommandAsync(string name)
        {
            return SendCommandAsync(new StepCommandArgs(name));
        }

        public async Task SendCommandAsync(StepCommandArgs commandArgs)
        {
            await _audit.AuditAsync(ObjectId, commandArgs, CurrentStepName);
            var step = await GetCurrentStepAsync();
            var result = await step.CommandAsync(commandArgs);
            switch (result.Action)
            {
                case StepCommandAction.ToStep:
                    await MoveToStep(async () =>
                    {
                        ValidateStepName(result.Result.StepName);
                        var enterCommand = new StepCommandArgsEnter(result.Result.Args, step.Name);
                        await _audit.AuditAsync(ObjectId, commandArgs, step.Name, result.Result.StepName, enterCommand);
                        await SaveCurrentStepAsync();
                        await UpdateCurrentStepAsync(result.Result.StepName);
                        step = await GetCurrentStepAsync();
                        await step.CommandAsync(enterCommand);
                        await SaveCurrentStepAsync();
                    });
                    break;
                case StepCommandAction.None:
                default:
                    break;
            }
        }

        public async ValueTask<IProcessStep<T>> GetCurrentStepAsync()
        {
            if (_currentStep == null)
            {
                _currentStep = StepFromStepName(CurrentStepName);
                if (_currentStep.StateType != null)
                {
                    var stepData = await StepRepository.GetCurrentStepDataAsync(_currentStep.StateType);
                    _currentStep.SetState(stepData);
                }
            }

            return _currentStep;
        }
        public IEnumerable<string> StepNames => Steps.Select(s => s.Name);

        public abstract string StartStepName { get; }

        public string CurrentStepName => StepRepository.CurrentStepName ?? StartStepName;

        public virtual ValueTask<bool> IsAllowedAsync(StepCommandDescriptor descriptor)
        {
            return descriptor.IsAllowedAsync(Array.Empty<string>());
        }

        #endregion

        #region IObjectContext

        public T Context { get; private set; }

        public async Task SetContextAsync(int id)
        {
            if (IsUsing(Context, id))
            {
                return;
            }

            var use = (await LoadAsync(id))
                ?? throw new BusinessLogicException($"Object not found Id={id}");

            await SetContextAsync(use);
        }

        public async Task SetContextAsync(T context)
        {
            if (_stepRepository != null)
            {
                var typename = GetType().Name;
                throw new InvalidOperationException($"Процесс {typename} уже инициализирован {_stepRepository.ProcessId}");
            }

            _stepRepository = await _processRepository.UseProcessAsync(context);
            OnLoaded();

            Context = context;
        }

        protected abstract bool IsUsing(T use, int id);

        protected abstract Task<T> LoadAsync(int id);

        #endregion

        protected virtual IProcessStep<T> StepFromStepName(string name)
        {
            if (!ValidateStepName(name, false))
            {
                name = null;
            }

            name = name ?? StartStepName;
            var step = (IProcessStep<T>)Activator.CreateInstance(Steps.First(t => t.Name == name), this);
            return step;
        }

        protected virtual void OnLoaded()
        {
        }

        protected virtual Task MoveToStep(Func<Task> moveAction)
        {
            //todo begin transaction
            return moveAction?.Invoke();
            //end transaction
        }

        protected bool ValidateStepName(string stepName, bool throwOnExcepton = true)
        {
            if (stepName != null && !StepNames.Contains(stepName))
            {
                if (throwOnExcepton)
                {
                    throw new ArgumentOutOfRangeException($"{nameof(stepName)}:{stepName}");
                }

                return false;
            }

            return true;
        }

        private async ValueTask SaveCurrentStepAsync()
        {
            var step = await GetCurrentStepAsync();
            if (_currentStep.StateType != null)
            {
                var stepData = _currentStep.GetState();
                await StepRepository.SaveStepDataAsync(stepData);
            }
        }

        private async ValueTask UpdateCurrentStepAsync(string stepName)
        {
            if (!StepNames.Contains(stepName))
            {
                throw new ArgumentOutOfRangeException($"Шаг с именем '{stepName}' не найден");
            }

            await StepRepository.UpdateCurrentStepNameAsync(stepName);
            _currentStep = null;
        }
    }
}
