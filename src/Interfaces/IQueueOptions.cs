using System.Collections.Generic;
using RabbitMQ.Client.Core.Options;

namespace RabbitMQ.Client.Core.Interfaces
{
    public interface IQueueOptions<TConsumerOrPublisher> where TConsumerOrPublisher: class
    {
        string QueueName { get; set; }
        bool Durable { get; set; }
        bool Exclusive { get; set; }
        bool AutoAck { get; set; }
        bool AutoDelete { get; set; }
        IDictionary<string,object> Arguments { get; set; }
        IEnumerable<ExchangeBindingOptions<TConsumerOrPublisher>> Bindings { get; set; }
    }
}
