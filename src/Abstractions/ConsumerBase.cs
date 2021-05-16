using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Core.Options;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Core.Abstractions
{
    public abstract class ConsumerBase<TConsumer> : BackgroundService where TConsumer : class
    {
        protected ConnectionFactory ConnectionFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly QueueOptions<TConsumer> _queueOptions;
        protected string Id;

        public ConsumerBase(
            IOptions<RabbitMQConnectionOptions> connectionOptions,
            IOptions<QueueOptions<TConsumer>> queueOptions)
        {
            ConnectionFactory = new ConnectionFactory
            {
                HostName = connectionOptions.Value.HostName,
                Port = connectionOptions.Value.Port,
                UserName = connectionOptions.Value.UserName,
                Password = connectionOptions.Value.Password,
                VirtualHost = connectionOptions.Value.VirtualHost
            };
            _connection = ConnectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _queueOptions = queueOptions.Value;
            Id = $"{Environment.UserName}-{typeof(TConsumer).Name}@{Guid.NewGuid()}";

            Setup();
        }

        protected void Setup()
        {
            _channel.QueueDeclare(
                queue: _queueOptions.QueueName,
                durable: _queueOptions.Durable,
                exclusive: _queueOptions.Exclusive,
                autoDelete: _queueOptions.AutoDelete
            );

            SetupExchangeBindings(_channel, _queueOptions);
        }

        public void Initialize()
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += OnReceived;
            consumer.Registered += Consumer_Registered;
            consumer.Unregistered += Consumer_Unregistered;
            _channel.BasicConsume(
               _queueOptions.QueueName,
               _queueOptions.AutoAck,
               consumer
           );
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            this.Initialize();
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        private void Consumer_Unregistered(object sender, ConsumerEventArgs e)
        {
            Console.WriteLine($"Unregister {Id} -> {e.ConsumerTags.Aggregate((a, b) => $"{a};{b}")}");
        }

        private void Consumer_Registered(object sender, ConsumerEventArgs e)
        {
            Console.WriteLine($"Register {Id} -> {e.ConsumerTags.Aggregate((a, b) => $"{a};{b}")}");
        }

        protected void SetupExchangeBindings(IModel channel, QueueOptions<TConsumer> queueOptions)
        {
            foreach (var exchangeToBind in queueOptions.Bindings)
            {
                _channel.QueueBind(
                    exchange: exchangeToBind.Exchange,
                    queue: queueOptions.QueueName,
                    routingKey: exchangeToBind.RoutingKey
                );
            }
        }

        public abstract void OnReceived(object sender, Events.BasicDeliverEventArgs e);


    }
}
