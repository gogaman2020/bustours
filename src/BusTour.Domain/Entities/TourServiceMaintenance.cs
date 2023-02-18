using BusTour.Domain.Models;
using Infrastructure.Db.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Entities
{
    public class TourServiceMaintenance : BaseEntity
    {
        /// <summary>
        /// Идентификатор тура
        /// </summary>
        public int? TourId { get; set; }

        /// <summary>
        /// Длительность
        /// </summary>
        public TimeSpan? Duration { get; set; }

        [IgnoreField]
        public Tour Tour { get; set; }
    }
}
