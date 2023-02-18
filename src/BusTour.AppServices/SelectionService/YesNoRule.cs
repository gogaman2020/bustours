using BusTour.AppServices.SelectionService.Models.Actions;

namespace BusTour.AppServices.SelectionService
{
    /// <summary>
    /// Правило.
    /// </summary>
    public class YesNoRule
    {
        /// <summary>
        /// Действие для случая ответ на вопрос про столы на двоих - да, ответ на вопрос про столы на четверых - да.
        /// </summary>
        public BaseRuleAction ActionYesYes { get; set; }

        /// <summary>
        /// Действие для случая ответ на вопрос про столы на двоих - да, ответ на вопрос про столы на четверых - нет.
        /// </summary>
        public BaseRuleAction ActionYesNo { get; set; }

        /// <summary>
        /// Действие для случая ответ на вопрос про столы на двоих - нет, ответ на вопрос про столы на четверых - да.
        /// </summary>
        public BaseRuleAction ActionNoYes { get; set; }

        /// <summary>
        /// Действие для случая ответ на вопрос про столы на двоих - нет, ответ на вопрос про столы на четверых - нет.
        /// </summary>
        public BaseRuleAction ActionNoNo { get; set; }

        /// <summary>
        /// Признак, что правило пустое.
        /// </summary>
        public bool IsEmpty => ActionYesYes == null && ActionYesNo == null && ActionNoYes == null && ActionNoNo == null;

        /// <summary>
        /// Получение действия.
        /// </summary>
        /// <param name="checkResultsTwoSeats">Ответ на вопрос про столы на двоих.</param>
        /// <param name="checkResultsFourSeats">Ответ на вопрос про столы на четверых.</param>
        /// <returns></returns>
        public BaseRuleAction GetAction(bool checkResultsTwoSeats, bool checkResultsFourSeats)
        {
            return
                checkResultsTwoSeats
                    ? checkResultsFourSeats
                        ? ActionYesYes
                        : ActionYesNo
                    : checkResultsFourSeats
                        ? ActionNoYes
                        : ActionNoNo;
        }
    }
}
