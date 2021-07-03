using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Core.Options;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ.Client.Core.Abstractions
{
    ///<summary>
    /// Rabbit MQ Consumer Abstraction Wrapper
    ///</summary>
    public abstract class ConsumerAsyncBase<TConsumer> : BackgroundService where TConsumer : class
    {
        protected ConnectionFactory ConnectionFactory;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly QueueOptions<TConsumer> _queueOptions;

        private readonly ILogger<TConsumer> _logger;
        protected string Id;

        public ConsumerAsyncBase(
            IOptions<RabbitMQConnectionOptions> connectionOptions,
            IOptions<QueueOptions<TConsumer>> queueOptions,
            ILogger<TConsumer> logger)
        {
            ConnectionFactory = new ConnectionFactory
            {
                HostName = connectionOptions.Value.HostName,
                Port = connectionOptions.Value.Port,
                UserName = connectionOptions.Value.UserName,
                Password = connectionOptions.Value.Password,
                VirtualHost = connectionOptions.Value.VirtualHost,
                DispatchConsumersAsync = true
            };
            _connection = ConnectionFactory.CreateConnection();
            _channel = _connection.CreateModel();
            _queueOptions = queueOptions.Value;
            Id = $"{Environment.UserName}-{typeof(TConsumer).Name}@{Guid.NewGuid()}";
            _logger = logger;

            Setup();
        }

        protected void Setup()
        {
            _logger.LogDebug($"Declaring queue {_queueOptions.QueueName}");
            _channel.QueueDeclare(
                queue: _queueOptions.QueueName,
                durable: _queueOptions.Durable,
                exclusive: _queueOptions.Exclusive,
                autoDelete: _queueOptions.AutoDelete
            );
            _logger.LogDebug($"Queue declared and channel is open? {_channel.IsOpen}");
            SetupExchangeBindings(_channel, _queueOptions);
        }

        ///<summary>
        /// Initialize Queue consumption
        ///</summary>
        public void Initialize()
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += OnReceived;
            consumer.Registered += ConsumerRegistered;
            consumer.Unregistered += ConsumerUnregistered;
            _channel.BasicConsume(
               _queueOptions.QueueName,
               _queueOptions.AutoAck,
               consumer
           );
           _logger.LogDebug($"Consumer {Id} intialized!");
        }

        ///<summary>
        /// Execute consumption and keeps consumer attached to queue
        ///</summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Initialize();
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1));
            }
        }

        private Task ConsumerUnregistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogDebug($"Unregister {Id} -> {e.ConsumerTags.Aggregate((a, b) => $"{a};{b}")}");
            return Task.CompletedTask;
        }

        private Task ConsumerRegistered(object sender, ConsumerEventArgs e)
        {
            _logger.LogDebug($"Register {Id} -> {e.ConsumerTags.Aggregate((a, b) => $"{a};{b}")}");
            return Task.CompletedTask;
        }

        protected void SetupExchangeBindings(IModel channel, QueueOptions<TConsumer> queueOptions)
        {
            foreach (var exchangeToBind in queueOptions.Bindings)
            {
                _logger.LogDebug($"Binding {queueOptions.QueueName} to exchange {exchangeToBind.Exchange} with routing key {exchangeToBind.RoutingKey}");
                _channel.QueueBind(
                    exchange: exchangeToBind.Exchange,
                    queue: queueOptions.QueueName,
                    routingKey: exchangeToBind.RoutingKey
                );
            }
        }

        public abstract Task OnReceived(object sender, BasicDeliverEventArgs e);

    }
}
