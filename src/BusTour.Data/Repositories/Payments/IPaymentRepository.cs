using BusTour.Domain.Entities;
using Infrastructure.Db.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace BusTour.Data.Repositories.Payments
{
    public interface IPaymentRepository : ICrudRepository<Payment>
    {
    }
}
