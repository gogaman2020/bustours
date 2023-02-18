using System.Collections.Generic;
using System.Linq;

namespace BusTour.AppServices.SelectionService
{
    /// <summary>
    /// Коллекция объектов подбора правил
    /// </summary>
    public class RuleSelectorWrapper
    {
        /// <summary>
        /// Объекты подбора правил с указанием диапазона количества гостей.
        /// </summary>
        public List<RuleSelectorRange> Selectors { get; } = new List<RuleSelectorRange>();

        /// <summary>
        /// Получение объекта подбора правил по указанному количеству гостей.
        /// </summary>
        /// <param name="selectedCount">Количество гостей.</param>
        /// <returns>Объект для подбора правил.</returns>
        public RuleSelector GetSelector(int selectedCount)
        {
            var result = Selectors.FirstOrDefault(p => p.FromGuestCount <= selectedCount && (p.ToGuestCount == null || p.ToGuestCount >= selectedCount));

            return result?.Selector;
        }

        /// <summary>
        /// Добавление объекта подбора правил с указанием диапазона количества гостей. 
        /// </summary>
        /// <param name="from">Минимальное количество гостей.</param>
        /// <param name="to">Максимальное количество гостей.</param>
        /// <param name="selector">Объект для подбора правил.</param>
        public void Add(int from, int? to, RuleSelector selector)
        {
            Selectors.Add(new RuleSelectorRange { FromGuestCount = from, ToGuestCount = to, Selector = selector });
        }
    }
}
