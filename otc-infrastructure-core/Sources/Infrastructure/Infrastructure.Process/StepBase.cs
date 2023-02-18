using Infrastructure.Common.Exceptions;
using Infrastructure.Process.Args;
using Infrastructure.Process.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Process
{
    /// <summary>
    /// Базовый шаг процесса
    /// </summary>
    /// <typeparam name="TP"></typeparam>
    /// <typeparam name="TS"></typeparam>
    public abstract class StepBase<T> : IProcessStep<T>
    {
        //пусть инициализируется только если нужно
        private readonly Lazy<StepCommandDescriptor[]> _lazyCommands;

        public StepBase(IProcess<T> process, string name = null, string nextStepName = null, string returnStepName = null, string cancelStepName = null)
        {
            Process = process;
            Name = name ?? GetType().Name;
            NextStepName = nextStepName;
            ReturnStepName = returnStepName;
            CancelStepName = cancelStepName;

            _lazyCommands = new Lazy<StepCommandDescriptor[]>(() => FillCommandDescriptors().ToArray());
        }

        public IProcess<T> Process { get; }

        protected abstract IEnumerable<StepCommandDescriptor> FillCommandDescriptors();
        //     => new[]
        // {
        //     new StepCommandDescriptor( new StepCommandNext<TP,TS>((TS)this), "Далее"),
        //     new StepCommandDescriptor( new StepCommandCancel<TP,TS>((TS)this), "Отмена"),
        //     new StepCommandDescriptor( new StepCommandReturn<TP,TS>((TS)this), "Возврат"),
        // };

        public string Name { get; }
        public string NextStepName { get; }
        public string ReturnStepName { get; }
        public string CancelStepName { get; }

        public virtual IEnumerable<StepCommandDescriptor> CommandDescriptors => _lazyCommands.Value;

        protected ValueTask<StepResult> Result(StepCommandAction action, StepCommandResult result)
        {
            return new ValueTask<StepResult>(new StepResult(action, result));
        }

        public virtual async ValueTask<StepResult> CommandAsync(StepCommandArgs commandArgs)
        {
            var descriptor = CommandDescriptors.FirstOrDefault(d => d.CommandName == commandArgs.Name);
            if (descriptor == null)
            {
                //опциональная команда,
                if (commandArgs.Name == StepCommands.Enter)
                {
                    await ExecuteEnterAsync(commandArgs);
                    return await Result(StepCommandAction.None, null);
                }

                throw new BusinessLogicException($"Команда '{commandArgs}' не доступна в текущем состоянии");
            }

            //пока всем всё можно
            var allowed = true; // await Process.IsAllowedAsync(descriptor);
            if (!allowed)
            {
                throw new BusinessLogicException($"Команда '{descriptor.CommandNameInMessage}' не доступна.");
            }

            var valid = await descriptor.Command.IsValidAsync();
            if (!valid.IsValid)
            {
                throw new BusinessLogicException(valid.Reason);
            }

            var stepResult = await descriptor.Command.ExecuteAsync(commandArgs);

            StepResult result;
            if (string.IsNullOrEmpty(stepResult.StepName))
            {
                result = await Result(StepCommandAction.None, null);
            }
            else
            {
                await ExecuteLeaveAsync(commandArgs);
                result = await Result(StepCommandAction.ToStep, stepResult);
            }

            return result;
        }

        /// <summary>
        /// Логика при заходе на шаг
        /// </summary>
        /// <param name="commandArgs"></param>
        /// <returns></returns>
        protected virtual Task ExecuteEnterAsync(StepCommandArgs commandArgs)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Логика при выходе из шага
        /// </summary>
        /// <param name="commandArgs"></param>
        /// <returns></returns>
        protected virtual Task ExecuteLeaveAsync(StepCommandArgs commandArgs)
        {
            return Task.CompletedTask;
        }

        #region State

        public virtual Type StateType => null;

        public virtual object GetState()
        {
            return null;
        }

        public virtual void SetState(object state)
        {
            if (StateType == null || GetState() == null || state == null)
            {
                return;
            }

            var s = GetState();
            foreach (var propertyInfo in StateType.GetProperties())
            {
                propertyInfo.SetValue(s, propertyInfo.GetValue(state));
            }
        }

        #endregion
    }
}
