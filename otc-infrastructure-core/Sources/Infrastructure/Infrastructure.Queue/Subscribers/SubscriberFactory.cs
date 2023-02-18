using System;
using Infrastructure.Common.Configs;
using Infrastructure.Common.DI;
using Infrastructure.Common.Logging;
using Infrastructure.Queue.Configs;
using Infrastructure.Queue.Configs.Old;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Infrastructure.Queue.Subscribers
{
    [InjectAsSingleton]
    public class SubscriberFactory : ISubscriberFactory, IDisposable
    {
        private readonly ConnectionFactory _factory;
        private IConnection _connection; 
        private IModel _model;

        //словарь очередей и обменников для сообщений обработанных с ошибкой
        //private Dictionary<string, string> _dlxDict;

        /// <summary>
        /// Логгер
        /// </summary>
        protected readonly ILogger _logger;

        public IModel Model => _model ?? UseConnection();
        
        public SubscriberFactory(IConfig<ConnectionSettings> connectionSettings)
        {
            _logger = ApplicationLogging.CreateLogger("Queue");

            //_dlxDict = new Dictionary<string, string>();

            _factory = new ConnectionFactory()
            {
                HostName = connectionSettings.Value.Host,
                VirtualHost = connectionSettings.Value.VirtualHost,
                UserName = connectionSettings.Value.User,
                Password = connectionSettings.Value.Password,
                DispatchConsumersAsync = true
            };
        }

        public void Dispose()
        {
            if(_connection != null &&
                _connection.IsOpen)
            {
                _connection.Dispose();
            }

            if(_model != null &&
                _model.IsOpen)
            {
                _model.Dispose();
            }
        }

        /// <summary>
        /// Добавить подписчика.
        /// </summary>
        /// <typeparam name="T">Тип сообщения</typeparam>
        /// <param name="subscriber">Подписчик</param>
        /// <param name="messageConfig">Настройки для подписчика</param>
        /// <returns></returns>
        public IDisposable Subscribe<T>(ISubscriber<T> subscriber)
            where T : class
        {
            var messageConfig = RabbitMqSubscriberConfig.For<T>(subscriber.GetType());
            UseConnection();
            string queueName = messageConfig.QueueName;

            string dlxName = null;

            //если используется обменник для сообщений, которые были обработаны дважды с ошибкой, объявляем этот обменник и очередь для него
            if (messageConfig.UseDLX)
            {
                dlxName = GenerateDLXExchangeName(queueName);
                _model.ExchangeDeclare(dlxName, "direct", durable: true, autoDelete: false);

                var dlxQueueName = GenerateDLXQueueName(dlxName);
                _model.QueueDeclare(queue: dlxQueueName, durable: true, exclusive: false, autoDelete: false);
                _model.QueueBind(dlxQueueName, dlxName, string.Empty);
            }

            var exchange = messageConfig.ExchangeName;
            try
            {
                _model.ExchangeDeclare(exchange, "fanout", durable: true, autoDelete: false);
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Обменник {exchange} уже существует. ({ex.Message})");
            }

            var queueInfo = _model.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false);

            if (queueInfo == null)
            {
                throw new Exception("queueInfo = null");
            }

            var config = RabbitMqMessageConfig.For<T>();

            var consumer = config.OldMessage
                ? (AsyncDefaultBasicConsumer) new BaseConsumer<T>(_model, subscriber, messageConfig, dlxName)
                : new MqMessageConsumer<T>(_model, subscriber, messageConfig, dlxName);

            if (messageConfig.Limit != default)
            {
                _model.BasicQos(0, (ushort) messageConfig.Limit, false);
            }

            _model.QueueBind(queueName, exchange, string.Empty);

            var consumerTag = _model.BasicConsume(queueInfo.QueueName, false, consumer);

            var disconnecter = new Disconnecter(consumerTag, _model);

            return disconnecter;
        }

        public IDisposable Subscribe(ITopologyConfiguration configuration, IBasicConsumer consumer, string queueName)
        {
            UseConnection();
            configuration.Configure(_model);
            var consumerTag = _model.BasicConsume(queueName, false, consumer);

            return new Disconnecter(consumerTag, _model);
        }

        #region Helpers
        public IModel UseConnection()
        {
            if (_connection?.IsOpen != true)
            {
                _connection = _factory.CreateConnection();
                _model = _connection.CreateModel();
            }

            return _model;
        }

        private string GenerateDLXExchangeName(string queueName)
        {
            return $"{queueName}-DLX";
        }

        private string GenerateDLXQueueName(string dlxExchangeName)
        {
            return $"{dlxExchangeName}-Queue";
        }
        #endregion
    }
}
