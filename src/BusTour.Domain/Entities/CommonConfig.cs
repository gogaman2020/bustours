using Infrastructure.Db.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Domain.Entities
{
    public class CommonConfig : BaseEntity
    {
        public string Code { get; set; }

        public string Value { get; set; }
    }
}
