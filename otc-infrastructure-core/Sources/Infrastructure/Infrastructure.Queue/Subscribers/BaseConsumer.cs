using System;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Common.Helpers;
using Infrastructure.Common.Json;
using Infrastructure.Common.Logging;
using Infrastructure.Queue.Configs;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Infrastructure.Queue.Subscribers
{
    public class BaseConsumer<T> : AsyncDefaultBasicConsumer
        where T : class
    {
        private readonly RabbitMqSubscriberConfig _messageConfig;
        private readonly ISubscriber<T> _subscriber;
        private readonly IModel _model;
        private readonly string _dlxName;

        /// <summary>
        /// Логгер
        /// </summary>
        protected readonly ILogger _logger;

        public BaseConsumer(IModel model, ISubscriber<T> subscriber, RabbitMqSubscriberConfig messageConfig, string dlxName) : base(model)
        {
            _logger = ApplicationLogging.CreateLogger("BaseConsumer");

            _subscriber = subscriber;
            _messageConfig = messageConfig;
            _model = model;
            _dlxName = dlxName;
        }

        public override async Task HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered, 
            string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            string jsonMessage = string.Empty;

            try
            {
                jsonMessage = Encoding.UTF8.GetString(body.Span);
                _logger.LogInformation($"Принято сообщение {jsonMessage} из обменника ${exchange} с ключом маршрутизации {routingKey}");
                T message = jsonMessage.FromJson<T>();

                await _subscriber.ConsumeAsync(message);
                _model.BasicAck(deliveryTag, false);

                _logger.LogInformation($"Обработано сообщение {jsonMessage} из обменника ${exchange} с ключом маршрутизации {routingKey}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Произошла ошибка при обработке сообщения {jsonMessage} из обменника ${exchange}");

                //повторно отправляем это же сообщение, если произошла ошибка при обработке
                if(!redelivered)
                {
                    _model.BasicNack(deliveryTag, true, true);
                }
                else
                {
                    if ((_messageConfig?.UseDLX ?? false) && !string.IsNullOrEmpty(_dlxName))
                    {
                        //Подправил чтобы из главное очереди сообщение удалялось, по хорошему надо переделать на механизм DLX который в рэббите, но потребует
                        //Удаления очередей, пока вопрос на согласовании.
                        _logger.LogWarning($"Сообщение {jsonMessage} из обменника ${exchange} будет отправлено в DLX, так как превышен лимит отправок.");
                        _model.BasicPublish(_dlxName, string.Empty, body: body);
                        _model.BasicNack(deliveryTag, true, false);
                    }
                    else
                    {
                        _logger.LogWarning($"Сообщение {jsonMessage} из обменника ${exchange} не будет повторно отправлено, так как превышен лимит отправок.");
                    }
                }
            }
        }
    }
}
