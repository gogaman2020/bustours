using BusTour.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BusTour.AppServices.CommonConfigService
{
    public interface ICommonConfigService
    {
        Task<List<CommonConfig>> GetCommonConfigAsync();
    }
}
