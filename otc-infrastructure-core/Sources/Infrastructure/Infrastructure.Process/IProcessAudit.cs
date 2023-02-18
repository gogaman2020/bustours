using Infrastructure.Process.Args;
using System.Threading.Tasks;

namespace Infrastructure.Process
{
    /// <summary>
    /// Интерфейс репозитория для аудита
    /// </summary>
    public interface IProcessAudit
    {
        /// <summary>
        /// Сохраняет состояние в БД
        /// </summary>
        /// <param name="objectId">Объект процесса</param>
        /// <param name="commandArgs">Параметры команды</param>
        /// <param name="stepFrom">Текущий шаг</param>
        /// <param name="stepTo">Следующий шаг</param>
        /// <param name="toArgs">Параметры команды следующего шага</param>
        /// <returns></returns>
        Task AuditAsync(int? objectId, StepCommandArgs commandArgs, string stepFrom, string stepTo = null, StepCommandArgs toArgs = null);
    }
}
