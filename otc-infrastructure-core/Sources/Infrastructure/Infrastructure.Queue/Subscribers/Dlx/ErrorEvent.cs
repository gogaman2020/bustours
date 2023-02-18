using System;

namespace Infrastructure.Queue.Subscribers.Dlx
{
    public class ErrorEvent
    {
        public string MessageBody { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime Date { get; set; }
    }
}
