using System.Collections.Generic;

namespace RabbitMQ.Client.Core.Interfaces
{
    public interface IExchangeOption
    {
        public string ExchangeName { get; set; }
        public string Type { get; set; }
        public bool Durable { get; set; }
        public bool AutoDelete { get; set; }
        public IDictionary<string, object> Arguments { get; set; }
    }
}
