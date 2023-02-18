using BusTour.Domain.Enums;

namespace BusTour.Domain.Models.Bus
{
    /// <summary>
    /// Объект автобуса.
    /// </summary>
    public class BusObject
    { 
        /// <summary>
        /// Тип объекта.
        /// </summary>
        public BusObjectTypes Type { get; set; }

        /// <summary>
        /// ИД объекта.
        /// </summary>
        public int Id { get; set; }
    }
}
