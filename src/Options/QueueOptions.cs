using System.Collections.Generic;

namespace RabbitMQ.Client.Core.Options
{
    ///<summary>
    /// Representation of Queue options section for a consumer (<see cref="RabbitMQ.Client.Core.Abstractions.ConsumerBase" />) or a publisher (<see cref="RabbitMQ.Client.Core.Abstractions.ConsumerBase" />) 
    ///</summary>
    public class QueueOptions<TConsumerOrPublisherQueue> where TConsumerOrPublisherQueue: class
    {
        public const string QueueSectionName = "Queue";
        public string QueueName { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoAck { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string, object> Arguments { get; set; }
        public IEnumerable<ExchangeBindingOptions<TConsumerOrPublisherQueue>> Bindings { get; set; }
    }
}
