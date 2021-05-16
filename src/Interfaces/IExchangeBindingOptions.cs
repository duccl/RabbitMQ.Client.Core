namespace RabbitMQ.Client.Core.Interfaces
{
    public interface IExchangeBindingOptions
    {
        string Exchange { get; set; }
        string RoutingKey { get; set; }
    }
}
