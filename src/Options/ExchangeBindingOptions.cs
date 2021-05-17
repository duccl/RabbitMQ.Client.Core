using RabbitMQ.Client.Core.Interfaces;

namespace RabbitMQ.Client.Core.Options
{
    ///<summary>
    /// Reprentation of an Exchange Bind section of appsettings.json
    ///</summary>
    public class ExchangeBindingOptions<TConsumerOrPublisherClass>: IExchangeBindingOptions
    {
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
    }
}
