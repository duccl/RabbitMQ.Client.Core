namespace RabbitMQ.Client.Core.Options
{
    public class ExchangeBindingOptions<TConsumerOrPublisherClass>
    {
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
    }
}
