using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Common.Models.FinancialCalculator
{
    /// <summary>
    /// Модель для рассчета суммы спецификации.
    /// </summary>
    public class SpecificationSumModel
    {
        /// <summary>
        /// Цена за ед без НДС
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Цена за ед. с НДС
        /// </summary>
        public decimal PriceWithVat { get; set; }

        /// <summary>
        /// Ставка НДС
        /// </summary>
        public decimal VatRate { get; set; }

        /// <summary>
        /// Количество
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Итоговая сумма с НДС
        /// </summary>
        public decimal Summ { get; set; }

        /// <summary>
        /// Итоговая сумма НДС
        /// </summary>
        public decimal SummVat { get; set; }
    }
}
