using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Common.Json;
using Infrastructure.Common.Logging;
using Infrastructure.Queue.Configs;
using Infrastructure.Queue.Configs.Old;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using ExchangeType = RabbitMQ.Client.ExchangeType;

namespace Infrastructure.Queue.Subscribers.Dlx
{
    public class RetryMessageConsumerWithTopologyConfiguration<T> : AsyncDefaultBasicConsumer, ITopologyConfiguration
        where T : class
    {
        RabbitMqSubscriberConfig _messageConfig;

        private string _dlxExchangeName;

        private bool _useCommonDlx;

        private ISubscriber<T> _subscriber;

        private IModel _model;

        /// <summary>
        /// Логгер
        /// </summary>
        protected readonly ILogger _logger;

        public RetryMessageConsumerWithTopologyConfiguration(RabbitMqSubscriberConfig messageConfig, ISubscriber<T> subscriber)
        {
            if (string.IsNullOrEmpty(messageConfig.ExchangeName))
                throw new ArgumentException("settings.ExchangeName");

            if (string.IsNullOrEmpty(messageConfig.QueueName))
                throw new ArgumentException("settings.QueueName");

            _logger = ApplicationLogging.CreateLogger("BaseConsumer");
            
            _messageConfig = messageConfig;
            _subscriber = subscriber;

            _useCommonDlx = !string.IsNullOrEmpty(messageConfig.CommonDlxExchange);
            _dlxExchangeName = _useCommonDlx ? messageConfig.CommonDlxExchange : $"{messageConfig.ExchangeName}-Dlx";
        }

        public void Configure(IModel model)
        {
            _model = model;

            var retryExchangeName = $"{_messageConfig.ExchangeName}-Retry";
            var retryQueueName = $"{_messageConfig.QueueName}-Retry";
            var queueExchangeType = ExchangeType.Fanout;
            var queueRoutingKey = "#";

            _model.ExchangeDeclare(_messageConfig.ExchangeName, queueExchangeType, durable: true, autoDelete: false);
            _model.ExchangeDeclare(retryExchangeName, queueExchangeType, durable: true, autoDelete: false);

            var queueArgs = new Dictionary<string, object>
            {
                { Headers.XDeadLetterExchange, retryExchangeName },
            };
            var retryQueueArgs = new Dictionary<string, object>
            {
                { Headers.XDeadLetterExchange, _messageConfig.ExchangeName },
                { Headers.XMessageTTL, _messageConfig.Ttl },
            };

            _model.QueueDeclare(_messageConfig.QueueName, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
            _model.QueueDeclare(retryQueueName, durable: true, exclusive: false, autoDelete: false, arguments: retryQueueArgs);
            _model.QueueBind(_messageConfig.QueueName, _messageConfig.ExchangeName, queueRoutingKey);
            _model.QueueBind(retryQueueName, retryExchangeName, queueRoutingKey);

            if (_messageConfig.Limit != default(int))
                _model.BasicQos(0, (ushort)_messageConfig.Limit, false);

            if (_messageConfig.UseDLX)
            {
                if (_useCommonDlx)
                {
                    _model.ExchangeDeclare(_dlxExchangeName, ExchangeType.Direct, durable: true, autoDelete: false);
                }
                else
                {
                    var dlxQueueName = $"{_messageConfig.QueueName}-Dlx";
                    _model.ExchangeDeclare(_dlxExchangeName, ExchangeType.Direct, durable: true, autoDelete: false);
                    _model.QueueDeclare(dlxQueueName, durable: true, exclusive: false, autoDelete: false);
                    _model.QueueBind(dlxQueueName, _dlxExchangeName, string.Empty);
                }
            }
        }

        public override async Task HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, 
            string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            string jsonMessage = string.Empty;

            try
            {
                jsonMessage = Encoding.UTF8.GetString(body.Span);
                T message = jsonMessage.FromJson<T>();

                await _subscriber.ConsumeAsync(message);
                _model.BasicAck(deliveryTag, false);

                _logger.LogInformation($"Обработано сообщение {jsonMessage} из обменника ${exchange} с ключом маршрутизации {routingKey}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Произошла ошибка при обработке сообщения {jsonMessage} из обменника ${exchange}");

                var currentRetryCount = properties.GetXDeathHeaderValue(_messageConfig.QueueName);
                //Если указано количество повторов прежде чем изьять из обработки.
                if (_messageConfig.RetryCount <= currentRetryCount)
                {
                    if (_messageConfig.UseDLX)
                    {
                        var errorMessage = new ErrorEvent
                        {
                            MessageBody = jsonMessage,
                            Date = DateTime.UtcNow,
                            ErrorMessage = ex.Message
                        };
                        _model.BasicPublish(_dlxExchangeName, string.Empty, body: Encoding.UTF8.GetBytes(errorMessage.ToJson()));
                    }

                    _model.BasicAck(deliveryTag, true);
                }
                else
                {
                    _model.BasicNack(deliveryTag, true, false);
                }
            }
        }

    }
}
