using Infrastructure.Common.Json;
using Infrastructure.Db.Common;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Process
{
    /// <summary>
    /// Состояние процесса
    /// </summary>
    [TableName(nameof(ProcessData))]
    public abstract class ProcessData: IEntity
    {
        public ProcessData()
        {
            StepsData = new Dictionary<string, string>();
        }

        /// <summary>
        /// ИД
        /// </summary>
        [TableField(nameof(Id))]
        public int Id { get; set; }

        /// <summary>
        /// ИД объекта
        /// </summary>
        [TableField(nameof(ObjectId), true)]
        public int ObjectId { get; set; }

        /// <summary>
        /// Текущий шаг
        /// </summary>
        [TableField(nameof(CurrentStepName))]
        public string CurrentStepName { get; set; }

        /// <summary>
        /// Параметры шагов
        /// </summary>
        [IgnoreField]
        public Dictionary<string, string> StepsData { get; }

        /// <summary>
        /// Шаги в JSON
        /// </summary>
        [TableField(nameof(StateJson))]
        public string StateJson
        {
            get
            {
                return StepsData
                    .ToDictionary(kv => kv.Key,
                        kv => kv.Value.FromJson<object>())
                    .ToJson();
            }
            set
            {
                StepsData.Clear();
                if (!string.IsNullOrEmpty(value))
                {
                    var dic = value.FromJson<Dictionary<string, object>>();
                    if (dic != null)
                    {
                        foreach (var keyValuePair in dic)
                        {
                            StepsData[keyValuePair.Key] = keyValuePair.Value.ToJson();
                        }
                    }
                }
            }
        }
    }
}
