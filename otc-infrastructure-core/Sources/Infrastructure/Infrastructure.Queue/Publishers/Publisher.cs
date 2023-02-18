using System;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Common.DI;
using Infrastructure.Common.Json;
using Infrastructure.Common.Logging;
using Infrastructure.Queue.Configs;
using Infrastructure.Queue.Model;
using Infrastructure.Queue.Subscribers;
using Infrastructure.Security.Models;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Infrastructure.Queue.Publishers
{
    public class Publisher<T> : IPublisher<T>
        where T : class
    {
        private readonly string _exchangeName;
        public readonly string _exchangeType;
        public readonly string _routingKey;
        private readonly SubscriberFactory _subscriberFactory;
        private readonly bool _oldPublisher;
        private readonly Scoped<IUserInfo> _userInfoScoped;
        private readonly RabbitMqMessageConfig _config;
        private IBasicProperties _messageProperties;
        
        private bool _inited;

        /// <summary>
        /// Логгер
        /// </summary>
        protected readonly ILogger _logger;

        public Publisher(SubscriberFactory subscriberFactory, RabbitMqMessageConfig config = null)
        {
            _logger = ApplicationLogging.CreateLogger("Queue");
            _subscriberFactory = subscriberFactory;
            _config = config ?? RabbitMqMessageConfig.For<T>();
            _exchangeName = _config.ExchangeName;
            _oldPublisher = _config.OldMessage;
            _exchangeType = _config.ExchangeType;
            _routingKey = _config.RoutingKey;

            _userInfoScoped = IoC.GetRequiredService<Scoped<IUserInfo>>();
        }

        public Task PublishAsync(T message)
        {
            Init();
            if (_oldPublisher)
            {
                Publish(message, _routingKey);
            }
            else
            {
                Publish(message);
            }
            return Task.CompletedTask;
        }

        private void Publish(T message, string routingKey)
        {
            var jsonMessage = message.ToJson();
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            try
            {
                _subscriberFactory.Model.BasicPublish(_exchangeName, routingKey ?? string.Empty, _messageProperties, body: body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка отправки сообщения {jsonMessage} в обменник {_exchangeName}.");
            }
        }

        private void Publish(T message)
        {
            var letter = new MqMessageT<T>
            {
                C = MqMessageT<T>.Key,
                Data = message,
                OrganizationId = _userInfoScoped.ServiceRequired.UserInfo.OrganizationId,
            };
            
            var jsonMessage = letter.ToJson();
            var body = Encoding.UTF8.GetBytes(jsonMessage);

            try
            {
                _subscriberFactory.Model.BasicPublish(_exchangeName, string.Empty, _messageProperties, body: body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка отправки сообщения {jsonMessage} в обменник {_exchangeName}.");
            }
        }

        private void Init()
        {
            if (_inited)
            {
                return;
            }

            try
            {
                _subscriberFactory.Model.ExchangeDeclare(_exchangeName, _exchangeType, durable: true, autoDelete: false);
                _messageProperties = GetMessageProps();
                _inited = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Ошибка создания обменника {_exchangeName}.");
            }
        }

        private IBasicProperties GetMessageProps()
        {
            var messageProperties = _subscriberFactory.Model.CreateBasicProperties();
            if (_config.PersistentMessage)
            {
                messageProperties.Persistent = true;
            }

            return messageProperties;
        }
    }
}