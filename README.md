# RabbitMQ.Client.Core
.NET Wrappers Abstractions and Dependency Injection for RabbitMQ

# Usage
Before all you need to setup your `appsettings.json` with the [`RabbitMQConnection`](src/Options/RabbitMQConnectionOptions.cs), [`ExchangesOption`](src/Options/ExchangesOption.cs) and your Publishers/Consumers sections.

## RabbitMQConnection Configuration
Access your `appsetting.json` and insert a section with the same schema below, you can change the properties values as needed. 

```json
"RabbitMQConnection": {
    "HostName": "localhost",
    "Port": 5672,
    "Username": "guest",
    "Password": "guest",
    "VirtualHost": "/"
}
```

It represents the [`RabbitMQConnectionOptions`](src/Options/RabbitMQConnectionOptions.cs) that is used to provide the connection settings to the consumers, publishers and the exchange configurator.

After this, just add the following line on your `ConfigureServices` target method and insert the `using RabbitMQ.Client.Core.Extensions`.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    services.AddRazorPages();
    ....
    services.AddMQConfigBindindgs(Configuration);
}
```

And now, to inject on the class that depends on it

```csharp
public MySampleClass(IOptions<RabbitMQConnectionOptions> connectionOptions,...)
{
    ....
}
```

## ExchangesOption Configuration

Like the example above, an insertion of the schema `ExchangesOption` is required at `appsettings.json`. 

```json
{
  "RabbitMQConnection": {...},
  ...
  "ExchangesOption": {
      "Exchanges": [
        {
          "ExchangeName": "my-1-topic-exchange",
          "Type": "topic",
          "Durable": true,
          "AutoDelete": true
        },
        ...
        {
          "ExchangeName": "my-n-direct-exchange",
          "Type": "direct",
          "Durable": false,
          "AutoDelete": true
        }
      ]
    }
}
```

It represents the [`ExchangesOption`](src/Options/ExchangesOption.cs) that create and configure the Rabbit MQ Exchanges.

After this, just add the following line on your `ConfigureServices` in sequence of `services.AddMQConfigBindindgs(Configuration);`

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    services.AddRazorPages();
    ....
    services.AddMQConfigBindindgs(Configuration);
    services.AddMQExchanges();
}
```

And voilà! The exchanges defined at `appsettings.json` are created at Rabbit MQ Broker.

# Publisher Configuration

Firstly you have to define a publisher class that inherits from [`PublisherBase`](src/Abstractions/PublisherBase.cs) like the example below.

```csharp
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Core.Abstractions;
using RabbitMQ.Client.Core.Options;
using System.Text;

namespace Example.Rabbit
{
    public class Publisher : PublisherBase<Publisher>
    {
        public Publisher(
            IOptions<RabbitMQConnectionOptions> connectionOptions,
            IOptions<ExchangeBindingOptions<Publisher>> exchangeBindingOptions, 
            ILogger<PublisherBase<Publisher>> logger) : base(connectionOptions, exchangeBindingOptions, logger)
        {
        }

        public override void Send<T>(T messageToSend)
        {
            Send(Encoding.Default.GetBytes(messageToSend.ToString()));
        }
    }
}
```

Also there is one more `Send` overloaded method, that allows you to use a custom routing key and exchange to publish. 

After this add it section __with the same name of your class__ to `appsettings.json`.

```json
{
  "RabbitMQConnection": {...},
  ...
  "ExchangesOption": {...},
  ...
  "Publisher":{
    "Exchange": "my-1-topic-exchange",
    "RoutingKey": "super.message.1"
  }
}
```

And then, after the statements `services.AddMQConfigBindindgs(Configuration)` and `services.AddMQExchanges()`, register it exchange configuration binding at `ConfigureServices` and itself.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    services.AddRazorPages();
    ....
    services.AddMQConfigBindindgs(Configuration);
    services.AddMQExchanges();
    services.AddPublisherExchange<Publisher>(Configuration);
    services.AddSingleton<Publisher>();
}
```

So now, you can inject your publisher into the class that depends on it and call one of the `Send` methods.

```csharp
public class Greeter
{
  private readonly Publisher _publisher;

  public Greeter(Publisher publisher)
  {
    _publisher = publisher;
  }

  public void SayHelloToRabbitMQ()
  {
    _publisher.Send<string>("Hello World!");
  }

}
```

# Consumer Configuration

Firstly you have to define a publisher class that inherits from [`ConsumerBase`](src/Abstractions/ConsumerBase.cs) like the example below.

```csharp
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RabbitMQ.Client.Core.Abstractions;
using RabbitMQ.Client.Core.Options;
using System.Text;
using System;

namespace Example.Rabbit
{
    public class Consumer : ConsumerBase<Consumer>
    {
        public Consumer(IOptions<RabbitMQConnectionOptions> connectionOptions,
                        IOptions<QueueOptions<Consumer>> queueOptions, 
                        ILogger<ConsumerBase<Consumer>> logger) : base(connectionOptions, queueOptions, logger)
        {}

        public override void OnReceived(object sender, RabbitMQ.Client.Events.BasicDeliverEventArgs e)
        {
            Console.WriteLine(Encoding.Default.GetString(e.Body.ToArray()));
        }
    }
}
```

After this add it section __with the same name of your class__ to `appsettings.json` with the properties that you need.

```json
{
  "RabbitMQConnection": {...},
  ...
  "ExchangesOption": {...},
  ...
  "Consumer": {
    "Queue": {
      "QueueName": "hazard-messages",
      "Durable": true,
      "Exclusive": false,
      "Autoack": true,
      "AutoDelete": false,
      "Bindings": [
        {
          "Exchange": "my-1-topic-exchange",
          "RoutingKey": "super.#"
        },
        ...
        {
          "Exchange": "my-n-direct-exchange",
          "RoutingKey": "something"
        }
      ]
    }
  },
  ...
  "Publisher":{...}
}
```

> The Queue section properties are defined at [`QueueOptions`](src/Options/QueueOptions.cs)

And then, after the statements `services.AddMQConfigBindindgs(Configuration)` and `services.AddMQExchanges()`, just register it with .

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    services.AddRazorPages();
    ....
    services.AddMQConfigBindindgs(Configuration);
    services.AddMQExchanges();
    ...
    services.AddConsumerQueue<Consumer>(hostContext.Configuration);
    ...
}
```

The consumer base class extends from `BackgroundService`, so when the `AddConsumerQueue` is called it already does the register of the consumer as a `HostedService`.

If you want to manually do the configuration, the `AddConsumerQueueBindings` needs to be used and also the consumer needs to be registered as `HostedService`. 

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddControllersWithViews();
    services.AddRazorPages();
    ....
    services.AddMQConfigBindindgs(Configuration);
    services.AddMQExchanges();
    ...
    services.AddConsumerQueueBindings<Consumer>(hostContext.Configuration);
    ...
    services.AddHostedService<Consumer>(hostContext.Configuration);
    ...
}
```

# Contributing

Open a branch, code and do a PR to main. For now this is the main flow.

# License

Licensed under the [MIT License](https://www.mit.edu/~amini/LICENSE.md).
