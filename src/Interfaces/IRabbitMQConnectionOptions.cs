namespace RabbitMQ.Client.Core.Interfaces
{
    public interface IRabbitMQConnectionOptions
    {
        string HostName { get; set; }
        int Port { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string VirtualHost { get; set; }
    }
}
