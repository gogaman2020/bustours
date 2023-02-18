namespace BusTour.Domain.Enums
{
    /// <summary>
    /// Статусы заказа.
    /// </summary>
    public enum OrderState
    {
        /// <summary>
        /// Черновик
        /// </summary>
        Draft = 10,

        /// <summary>
        /// Ожидает оплаты
        /// </summary>
        WaitingForPayment = 20,

        /// <summary>
        /// Оплачен
        /// </summary>
        Paid = 30,

        /// <summary>
        /// Не оплачен
        /// </summary>
        NotPaid = 40,

        /// <summary>
        /// Отменён
        /// </summary>
        Canceled = 50,
    }
}
