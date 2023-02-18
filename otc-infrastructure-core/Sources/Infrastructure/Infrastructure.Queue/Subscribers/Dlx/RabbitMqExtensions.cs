using System.Collections.Generic;
using System.Text;
using RabbitMQ.Client;

namespace Infrastructure.Queue.Subscribers.Dlx
{
    public static class RabbitMqExtensions
    {
        public static long GetXDeathHeaderValue(this IBasicProperties properties, string queueName)
        {
            var xDeathHeaderName = "x-death";

            object xDeathHeaderObj = null;

            if (string.IsNullOrEmpty(queueName) || properties.Headers.TryGetValue(xDeathHeaderName, out xDeathHeaderObj))
            {
                if (xDeathHeaderObj != null && xDeathHeaderObj is List<object>)
                {
                    var deathProperties = (List<object>)xDeathHeaderObj;
                    bool endFind = false;
                    var i = 0;
                    var count = deathProperties.Count;
                    while (!endFind || i <= (count - 1))
                    {
                        var prop = (Dictionary<string, object>)deathProperties[i];
                        var queueAsByteArray = (byte[])prop["queue"];
                        var findQueueName = Encoding.Default.GetString(queueAsByteArray);
                        if(findQueueName.ToLower().Trim() == queueName.ToLower().Trim())
                        {
                            return (long)prop["count"];
                        }
                        ++i;
                    }
                }
            }

            return 0;
        }
    }
}
