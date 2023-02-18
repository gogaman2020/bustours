using System;
using RabbitMQ.Client;

namespace Infrastructure.Queue.Subscribers
{
    public class Disconnecter : IDisposable
    {
        private readonly string _consumerTag;
        private readonly IModel _model;

        public Disconnecter(string consumerTag, IModel model)
        {
            _consumerTag = consumerTag;
            _model = model;
        }

        public void Dispose()
        {
            _model.BasicCancel(_consumerTag);
        }
    }
}
