using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client.Core.Abstractions;
using RabbitMQ.Client.Core.Configurations;
using RabbitMQ.Client.Core.Interfaces;
using RabbitMQ.Client.Core.Options;

namespace RabbitMQ.Client.Core.DependencyInjection.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMQExchanges(this IServiceCollection services)
        {
            services.AddSingleton<IExchangesConfigurator, ExchangesConfigurator>();
            services.BuildServiceProvider().GetRequiredService<IExchangesConfigurator>()?.Setup();
            return services;
        }

        public static IServiceCollection AddConsumerQueueBindings<TConsumer>(this IServiceCollection services, IConfiguration configuration) where TConsumer: class
        {
            services.Configure<QueueOptions<TConsumer>>(
                configuration.GetSection(
                    typeof(TConsumer).Name
                ).GetSection(QueueOptions<TConsumer>.QueueSectionName)
            );
            return services;
        }

        public static IServiceCollection AddConsumerQueue<TConsumer>(this IServiceCollection services, IConfiguration configuration) where TConsumer: ConsumerBase<TConsumer>
        {
            services.AddConsumerQueueBindings<TConsumer>(configuration);
            services.AddSingleton<TConsumer>()
                .AddHostedService<TConsumer>();
            return services;
        }

        public static IServiceCollection AddAsyncConsumerQueue<TConsumer>(this IServiceCollection services, IConfiguration configuration) where TConsumer : ConsumerAsyncBase<TConsumer>
        {
            services.AddConsumerQueueBindings<TConsumer>(configuration);
            services.AddSingleton<TConsumer>()
                .AddHostedService<TConsumer>();
            return services;
        }

        public static IServiceCollection AddSigletonPublisher<TPublisher>(this IServiceCollection services, IConfiguration configuration) where TPublisher : PublisherBase<TPublisher>
        {
            services.AddPublisherExchange<TPublisher>(configuration);
            services.AddSingleton(typeof(TPublisher));
            return services;
        }

        public static IServiceCollection AddTransientPublisher<TPublisher>(this IServiceCollection services, IConfiguration configuration) where TPublisher : PublisherBase<TPublisher>
        {
            services.AddPublisherExchange<TPublisher>(configuration);
            services.AddTransient(typeof(TPublisher));
            return services;
        }

        public static IServiceCollection AddScopedPublisher<TPublisher>(this IServiceCollection services, IConfiguration configuration) where TPublisher : PublisherBase<TPublisher>
        {
            services.AddPublisherExchange<TPublisher>(configuration);
            services.AddScoped(typeof(TPublisher));
            return services;
        }

        public static IServiceCollection AddSigletonPublisher<TIPublisher, TPublisher>(this IServiceCollection services, IConfiguration configuration) where TPublisher : PublisherBase<TPublisher>
        {
            services.AddPublisherExchange<TPublisher>(configuration);
            services.AddSingleton(typeof(TIPublisher), typeof(TPublisher));
            return services;
        }

        public static IServiceCollection AddTransientPublisher<TIPublisher,TPublisher>(this IServiceCollection services, IConfiguration configuration) where TPublisher : PublisherBase<TPublisher>
        {
            services.AddPublisherExchange<TPublisher>(configuration);
            services.AddTransient(typeof(TIPublisher), typeof(TPublisher));
            return services;
        }

        public static IServiceCollection AddScopedPublisher<TIPublisher,TPublisher>(this IServiceCollection services, IConfiguration configuration) where TPublisher : PublisherBase<TPublisher>
        {
            services.AddPublisherExchange<TPublisher>(configuration);
            services.AddScoped(typeof(TIPublisher), typeof(TPublisher));
            return services;
        }

        public static IServiceCollection AddPublisherExchange<TPublisher>(this IServiceCollection services, IConfiguration configuration) where TPublisher : PublisherBase<TPublisher>
        {
            services.Configure<ExchangeBindingOptions<TPublisher>>(
                configuration.GetSection(
                    typeof(TPublisher).Name
                )
            );
            return services;
        }

        public static IServiceCollection AddMQConfigBindindgs(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMQConnectionOptions>(configuration.GetSection(RabbitMQConnectionOptions.RabbitMQConnection));
            services.Configure<ExchangesOption>(configuration.GetSection(ExchangesOption.ExchangesSectionName));
            return services;
        }

    }
}
