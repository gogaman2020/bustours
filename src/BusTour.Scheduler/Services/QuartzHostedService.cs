using BusTour.Scheduler.Helpers;
using BusTour.Scheduler.Options;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Quartz;
using Quartz.Spi;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BusTour.Scheduler.Services
{
	public class QuartzHostedService : IHostedService
	{
		private readonly ILogger _logger;
		private readonly ISchedulerFactory _schedulerFactory;
		private readonly IJobFactory _jobFactory;

		private readonly SchedulerConfig _schedulerConfig;

		public QuartzHostedService(
			ISchedulerFactory schedulerFactory,
			IJobFactory jobFactory,
			IOptions<SchedulerConfig> schedulerConfig)
		{
			_schedulerFactory = schedulerFactory;
			_jobFactory = jobFactory;

			_schedulerConfig = schedulerConfig.Value;

			_logger = ApplicationLogging.CreateLogger("QuartzHostedService");
		}
		public IScheduler Scheduler { get; set; }

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			if (_schedulerConfig.Enable)
			{
				Scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
				Scheduler.JobFactory = _jobFactory;

				foreach (var jobConfig in _schedulerConfig.Jobs)
				{
					var jobType = FindType(jobConfig.TypeName);

					if (jobType != null)
					{
						var job = CreateJob(jobType, jobConfig.Description);
						var trigger = CreateTrigger(jobType, jobConfig.CronExpression);

						await Scheduler.ScheduleJob(job, trigger, cancellationToken);
						await Scheduler.TriggerJob(job.Key);
					}
					else
					{
						_logger.LogWarning($"Работа {JsonConvert.SerializeObject(jobConfig)} не была запланирована, так как не найдена тип для конфигурации {jobConfig.TypeName}");
					}
				}

				await Scheduler.Start(cancellationToken);
			}
		}

		public async Task StopAsync(CancellationToken cancellationToken)
		{
			if (_schedulerConfig.Enable)
			{
				await Scheduler?.Shutdown(cancellationToken);
			}
		}

		private static IJobDetail CreateJob(Type jobType, string description)
		{
			return JobBuilder
				.Create(jobType)
				.WithIdentity(jobType.FullName)
				.WithDescription(description)
				.Build();
		}

		private static ITrigger CreateTrigger(Type jobType, string cronExpression)
		{
			return TriggerBuilder
				.Create()
				.WithIdentity($"{jobType.FullName}.trigger")
				.WithCronSchedule(cronExpression)
				.WithDescription(cronExpression)
				.Build();
		}

		private static Type FindType(string fullName)
		{
			return
				AppDomain.CurrentDomain.GetAssemblies()
					.Where(a => !a.IsDynamic)
					.SelectMany(a =>
					{
						try
						{
							return a.GetTypes();
						}
						catch
						{
							return Array.Empty<Type>();
						}
					})
					.FirstOrDefault(t => t.FullName.Equals(fullName));
		}
	}
}
