using Infrastructure.Common.Attributes;

namespace Infrastructure.Common.Enums
{
    /// <summary>
    /// Коды категорий рабочих групп
    /// </summary>
    public enum WorkgroupCategoryCode
    {
        /// <summary>
        /// Подразделение
        /// </summary>
        [Title("Подразделение")]
        Department = 1,

        /// <summary>
        /// Филиал
        /// </summary>
        [Title("Филиал")]
        Subsidiary = 2,

        /// <summary>
        /// Ответственный ОЗРК
        /// </summary>
        [Title("Ответственный ОЗРК")]
        ResponsiblesOZRK = 3,

        /// <summary>
        /// Ответственный ДТЗ ОДР
        /// </summary>
        [Title("Ответственный ДТЗ ОДР")]
        ResponsiblesDTZ_ODR = 4,

        /// <summary>
        /// Ответственный ЦО ОДР
        /// </summary>
        [Title("Ответственный ЦО ОДР")]
        ResponsiblesCO_ODR = 5,

        /// <summary>
        /// Ответственный ЦО ОЦМ
        /// </summary>
        [Title("Ответственный ЦО ОЦМ")]
        ResponsiblesCO_OCM = 6,

        /// <summary>
        /// Исполнители ЮС
        /// </summary>
        [Title("Исполнители ЮС")]
        ResponsiblesUS = 7,

        /// <summary>
        /// Ответственный ДТЗ ОСК
        /// </summary>
        [Title("Ответственный ДТЗ ОСК")]
        ResponsiblesDTZ_OSK = 8,

        /// <summary>
        /// Ответственный АСУ
        /// </summary>
        [Title("Ответственный АСУ")]
        Responsibles_ASU = 9,

        /// <summary>
        /// Ответственный КЛСХ
        /// </summary>
        [Title("Ответственный КЛСХ")]
        ResponsiblesKLSH = 10,

        /// <summary>
        /// Исполнитель ФЭС
        /// </summary>
        [Title("Исполнитель ФЭС")]
        ResponsiblesFES = 11
    }
}
