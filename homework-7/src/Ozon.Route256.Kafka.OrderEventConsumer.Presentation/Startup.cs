using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Extensions;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Providers;
using Ozon.Route256.Kafka.OrderEventConsumer.Domain.Repositories;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Common;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Repositories;
using Ozon.Route256.Kafka.OrderEventConsumer.Infrastructure.Settings;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Extensions;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Handlers;
using Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Providers;
using OrderEvent = Ozon.Route256.Kafka.OrderEventConsumer.Presentation.Contracts.OrderEvent;

namespace Ozon.Route256.Kafka.OrderEventConsumer.Presentation;

public sealed class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration) => _configuration = configuration;

    public void ConfigureServices(IServiceCollection services)
    {
        services
            .AddLogging();
        
        var connectionString = _configuration["ConnectionString"]!;
        services
            .AddFluentMigrator(
                connectionString,
                typeof(SqlMigration).Assembly);
        
        services.AddSingleton<IItemRepository, ItemRepository>(_ => new ItemRepository(connectionString));
        services.AddSingleton<IPaymentRepository, PaymentRepository>(_ => new PaymentRepository(connectionString));
        services.AddServiceExtensions();
        
        services.Configure<KafkaBackgroundOptions>(_configuration.GetSection(nameof(KafkaBackgroundOptions)));
        services.AddKafkaHandler<Ignore, OrderEvent, OrderEventHandler>(
            new NullTextJsonSerializer<Ignore>(),
            new SystemTextJsonSerializer<OrderEvent>(new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } }));
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
    }
}
