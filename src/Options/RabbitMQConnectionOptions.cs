using RabbitMQ.Client.Core.Interfaces;

namespace RabbitMQ.Client.Core.Options
{
    ///<summary>
    /// Representation of Rabbit Connection Section named "RabbitMQConnection" in appsetting.json
    ///</summary>
    public class RabbitMQConnectionOptions: IRabbitMQConnectionOptions
    {
        public const string RabbitMQConnection = "RabbitMQConnection";
        public string HostName { get; set; }
        public int Port { get; set; } = 5672;
        public string UserName { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; } = "";
    }
}
