using Infrastructure.Common.Exceptions;
using Infrastructure.Process.Args;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Process.Commands
{
    /// <summary>
    /// Описание команды
    /// </summary>
    public class StepCommandDescriptor
    {
        /// <summary>
        /// Хелпер для запуска исполнения команды
        /// </summary>
        private class ExecCommand : StepCommandBase
        {
            private readonly Func<StepCommandArgs, ValueTask<StepCommandResult>> _func;

            public ExecCommand(Func<StepCommandArgs, ValueTask<StepCommandResult>> func)
            {
                _func = func;
            }

            public override string Name => null;
            public override ValueTask<StepCommandResult> ExecuteAsync(StepCommandArgs commandArgs)
            {
                return _func(commandArgs);
            }
        }

        public StepCommandDescriptor(string commandName, Func<StepCommandArgs, ValueTask<StepCommandResult>> func, string commandNameInMessage, string[] permissions = null)
            : this(commandName, new ExecCommand(func), commandNameInMessage, permissions)
        {
        }

        public StepCommandDescriptor(StepCommandBase command, string commandNameInMessage, string[] permissions = null)
            : this(command.Name, command, commandNameInMessage, permissions)
        {
        }

        public StepCommandDescriptor(string commandName, IStepCommand command, string commandNameInMessage, string[] permissions = null)
        {
            CommandName = commandName ?? throw new BusinessLogicException("Имя команды не определено");
            Command = command ?? throw new BusinessLogicException($"Команда с именем {commandName} не определена");
            CommandNameInMessage = commandNameInMessage;
            Permissions = permissions;
        }

        /// <summary>
        /// Название команды
        /// </summary>
        public string CommandName { get; }

        /// <summary>
        /// Команда
        /// </summary>
        public IStepCommand Command { get; }

        /// <summary>
        /// Название команды для сообщений
        /// </summary>
        public string CommandNameInMessage { get; }

        /// <summary>
        /// Права доступа
        /// </summary>
        public string[] Permissions { get; }

        /// <summary>
        /// Проверка доступности
        /// </summary>
        /// <param name="permissions">Права доступа</param>
        /// <returns></returns>
        public ValueTask<bool> IsAllowedAsync(string[] permissions)
        {
            return (Permissions == null || Permissions.Intersect(permissions).Any())
                ? Command.IsAllowedAsync()
                : new ValueTask<bool>(false);
        }
    }
}
