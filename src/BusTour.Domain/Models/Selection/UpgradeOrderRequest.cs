using BusTour.Domain.Enums;
using BusTour.Domain.Models.Bus;
using System.Collections.Generic;

namespace BusTour.Domain.Models.Selection
{
    /// <summary>
    /// Запрос  на upgrade.
    /// </summary>
    public class UpgradeOrderRequest
    {
        /// <summary>
        /// Идентификатор заказа.
        /// </summary>
        public int OrderId { get; set; }

        /// <summary>
        /// Объект, по которому кликнул пользователь.
        /// </summary>
        public BusObject ClickedObject { get; set; }
    }
}
