using RabbitMQ.Client.Core.Interfaces;

namespace RabbitMQ.Client.Core.Options
{
    public class ExchangeBindingOptions<TConsumerOrPublisherClass>: IExchangeBindingOptions
    {
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
    }
}
