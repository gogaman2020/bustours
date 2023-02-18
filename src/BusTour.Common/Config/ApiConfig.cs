using Infrastructure.Common.Configs;
using System;

namespace BusTour.Common.Config
{
    [Config]
    public class ApiConfig
    {
        public int PaymentMinutes { get; set; }
        public TimeSpan DeadlineTime { get; set; }

        public string AdminEmail { get; set; }
    }
}
