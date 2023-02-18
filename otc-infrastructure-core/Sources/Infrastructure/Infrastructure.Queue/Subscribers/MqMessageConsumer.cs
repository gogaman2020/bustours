using System;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Common.DI;
using Infrastructure.Common.Json;
using Infrastructure.Queue.Configs;
using Infrastructure.Queue.Model;
using Infrastructure.Security.SecurityTokens;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Infrastructure.Queue.Subscribers
{
    public class MqMessageConsumer<T> : AsyncDefaultBasicConsumer
        where T : class
    {
        private readonly RabbitMqSubscriberConfig _messageConfig;
        private readonly ISubscriber<T> _subscriber;
        private readonly IModel _model;
        private readonly string _dlxName;
        private readonly IAuthenticationService _authService;
        private readonly RabbitMqSubscriberConfig _config;
        private readonly string _username;

        /// <summary>
        /// Логгер
        /// </summary>
        protected readonly ILogger<MqMessageConsumer<T>> _logger;

        public MqMessageConsumer(IModel model, ISubscriber<T> subscriber, RabbitMqSubscriberConfig messageConfig,
            string dlxName)
            : base(model)
        {
            _subscriber = subscriber;
            _messageConfig = messageConfig;
            _model = model;
            _dlxName = dlxName;
            _logger = IoC.GetRequiredService<ILogger<MqMessageConsumer<T>>>();
            _authService = IoC.GetRequiredService<IAuthenticationService>();
            _config = RabbitMqSubscriberConfig.For<T>(_subscriber.GetType());
            _username = "Subscriber." + _config.QueueName;
        }

        public override async Task HandleBasicDeliver(string consumerTag, ulong deliveryTag, bool redelivered,
            string exchange, string routingKey, IBasicProperties properties, ReadOnlyMemory<byte> body)
        {
            string jsonMessage = string.Empty;

            try
            {
                jsonMessage = Encoding.UTF8.GetString(body.Span);
                _logger.LogInformation(
                    $"Принято сообщение {jsonMessage} из обменника ${exchange} с ключом маршрутизации {routingKey}");
                var message = jsonMessage.FromJson<MqMessageT<T>>();
                if (message.C == MqMessageT<T>.Key)
                {
                    await _authService.RunAsSysUserAsync(_username, message.OrganizationId, null,
                        async sp => { await _subscriber.ConsumeAsync(message.Data); });
                }
                else
                {
                    var oldMessage = jsonMessage.FromJson<T>();
                    await _subscriber.ConsumeAsync(oldMessage);
                }

                _model.BasicAck(deliveryTag, false);

                _logger.LogInformation(
                    $"Обработано сообщение {jsonMessage} из обменника ${exchange} с ключом маршрутизации {routingKey}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    $"Произошла ошибка при обработке сообщения {jsonMessage} из обменника ${exchange}");

                //повторно отправляем это же сообщение, если произошла ошибка при обработке
                if (!redelivered)
                {
                    _model.BasicNack(deliveryTag, true, true);
                }
                else
                {
                    if ((_messageConfig?.UseDLX ?? false) && !string.IsNullOrEmpty(_dlxName))
                    {
                        //Подправил чтобы из главное очереди сообщение удалялось, по хорошему надо переделать на механизм DLX который в рэббите, но потребует
                        //Удаления очередей, пока вопрос на согласовании.
                        _logger.LogWarning(
                            $"Сообщение {jsonMessage} из обменника ${exchange} будет отправлено в DLX, так как превышен лимит отправок.");
                        _model.BasicPublish(_dlxName, string.Empty, body: body);
                        _model.BasicNack(deliveryTag, true, false);
                    }
                    else
                    {
                        _logger.LogWarning(
                            $"Сообщение {jsonMessage} из обменника ${exchange} не будет повторно отправлено, так как превышен лимит отправок.");
                    }
                }
            }
        }
    }
}