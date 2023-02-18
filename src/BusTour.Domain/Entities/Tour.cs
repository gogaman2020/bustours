using BusTour.Domain.Enums;
using BusTour.Domain.Helpers;
using Infrastructure.Db.Common;
using System;
using System.Collections.Generic;

namespace BusTour.Domain.Entities
{
    public class Tour : BaseEntity
    {
        /// <summary>
        /// Номер заказа
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// Дата и время отправления
        /// </summary>
        public DateTime Departure { get; set; }

        /// <summary>
        /// Длительность
        /// </summary>
        [IgnoreField]
        public TimeSpan? Duration => ServiceMaintenance?.Duration ?? PrivateHire?.Duration ?? Route?.Duration;

        /// <summary>
        /// Дата и время прибытия 
        /// </summary>
        [IgnoreField]
        public DateTime Arrival => Departure.Add(Duration ?? TimeSpan.Zero);

        /// <summary>
        /// Маршрут (id)
        /// </summary>
        public int? RouteId { get; set; }

        /// <summary>
        /// Цена места
        /// </summary>
        public decimal? SeatPrice { get; set; }

        /// <summary>
        /// Цена VIP места
        /// </summary>
        public decimal? VipPrice { get; set; }

        /// <summary>
        /// Скидка
        /// </summary>
        public decimal? Discount { get; set; }

        /// <summary>
        /// Тип тура
        /// </summary>
        public TourType? Type { get; set; }

        /// <summary>
        /// Маршрут
        /// </summary>
        [IgnoreField]
        public Route Route { get; set; }

        /// <summary>
        /// Автобус (id)
        /// </summary>
        public int BusId { get; set; }

        /// <summary>
        /// Поля частного заказа для тура
        /// </summary>
        [IgnoreField]
        public TourPrivateHire PrivateHire { get; set; }

        /// <summary>
        /// Поля сервисного тура
        /// </summary>
        [IgnoreField]
        public TourServiceMaintenance ServiceMaintenance { get; set; }

        /// <summary>
        /// Автобус
        /// </summary>
        [IgnoreField]
        public Bus Bus { get; set; }

        /// <summary>
        /// Коллекция меню в туре.
        /// </summary>
        [IgnoreField]
        public List<TourMenu> TourMenus { get; set; }

        /// <summary>
        /// Коллекция напитков в туре.
        /// </summary>
        [IgnoreField]
        public List<TourBeverage> TourBeverages { get; set; }

        [IgnoreField]
        public List<Table> Tables { get; set; }

        /// <summary>
        /// Статус тура
        /// </summary>
        public TourState? TourState => (TourState?)ProcessHelper.GetEnumItemByStepName(CurrentStepName);

        /// <summary>
        /// Текущий шаг(ключ) тура
        /// </summary>
        public string CurrentStepName { get; set; }

        /// <summary>
        /// Признак активного для заказа тура
        /// Вычисляемое, в БД не лежит
        /// </summary>
        public bool IsAvailableForBooking { get; set; }

        /// <summary>
        /// Количество занятых мест
        /// </summary>
        [IgnoreField]
        public int OccupiedSeatsCount { get; set; }

        /// <summary>
        /// Количество заказов
        /// </summary>
        [IgnoreField]
        public int? OrdersCount { get; set; }

        public Tour()
        {
            TourMenus = new List<TourMenu>();
            TourBeverages = new List<TourBeverage>();
            Tables = new List<Table>();
        }
    }
}
