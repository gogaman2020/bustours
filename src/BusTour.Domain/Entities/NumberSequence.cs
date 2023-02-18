using BusTour.Domain.Enums;
using Infrastructure.Db.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Entities
{
    /// <summary>
    /// Уведолмение
    /// </summary>
    public class NumberSequence : BaseEntity
    {
        /// <summary>
        /// Последовательность
        /// </summary>
        public string Sequence { get; set; }   

        /// <summary>
        /// Номер последовательности
        /// </summary>
        public int Number { get; set; }
    }
}
