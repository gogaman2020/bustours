using Infrastructure.Common.Configs;
using System.Collections.Generic;

namespace BusTour.Scheduler.Options
{
    [Config]
    public class SchedulerConfig
    {
        public bool Enable { get; set; }

        public List<JobConfig> Jobs { get; set; }
    }

    public class JobConfig
    {
        public string TypeName { get; set; }

        public string Description { get; set; }

        public string CronExpression { get; set; }
    }
}
