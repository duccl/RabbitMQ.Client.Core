using System.Collections.Generic;

namespace RabbitMQ.Client.Core.Interfaces
{
    public interface IQueueOptions<TSectionName> where TSectionName: class
    {
        public string QueueName { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoAck { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string,object> Arguments { get; set; }
        public IEnumerable<IExchangeBindingOptions> Bindings { get; set; }
    }
}
