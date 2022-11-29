using Autofac;
using Autofac.Extensions.DependencyInjection;
using EventBus;
using EventBus.Abstractions;
using EventBusRabbitMq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Reporting.Api.Infrastructure;
using Reporting.Api.Infrastructure.Repositories;
using Reporting.Api.IntegrationEvents;
using Reporting.Api.IntegrationEvents.EventHandling;
using StackExchange.Redis;
using System.Reflection;
using Reporting.Api.IntegrationEvents.Events;
using Reporting.Api.Services;

namespace Reporting.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var configuration = GetConfiguration();
        var builder = WebApplication.CreateBuilder(args);

        builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

        builder.Configuration.AddConfiguration(configuration);

        builder.Services.Configure<ReportingSettings>(configuration);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        #region Database Related Services

        builder.Services.AddEntityFrameworkNpgsql().AddDbContext<ReportingContext>(options =>
        {
            options.UseNpgsql(configuration["ConnectionString"], pgSqlOptions =>
            {
                pgSqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);
                pgSqlOptions.EnableRetryOnFailure(15, TimeSpan.FromSeconds(30), null);
            });
        });

        builder.Services.AddTransient<IReportRepository, PgSqlReportRepository>();

        #endregion

        builder.Services.AddHttpClient();

        builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        #region Message Broker Related Services

        builder.Services.AddSingleton(sp =>
        {
            var settings = sp.GetRequiredService<IOptions<ReportingSettings>>().Value;
            var redisConfiguration = ConfigurationOptions.Parse(settings.RedisConnectionString, true);

            return ConnectionMultiplexer.Connect(redisConfiguration);
        });

        builder.Services.AddTransient<IReportingIntegrationEventService, ReportingIntegrationEventService>();

        builder.Services.AddSingleton<IEventBus, EventBusRabbitMq.EventBusRabbitMq>(sp =>
        {
            var subscriptionClientName = configuration["SubscriptionClientName"];
            var rabbitMqPersistentConnection = sp.GetRequiredService<IRabbitMqPersistentConnection>();
            var iLifetimeScope = sp.GetRequiredService<ILifetimeScope>();
            var eventBusSubscriptionsManager = sp.GetRequiredService<IEventBusSubscriptionsManager>();

            var retryCount = 5;
            if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
            {
                retryCount = int.Parse(configuration["EventBusRetryCount"]);
            }

            return new EventBusRabbitMq.EventBusRabbitMq(rabbitMqPersistentConnection, iLifetimeScope, eventBusSubscriptionsManager, subscriptionClientName, retryCount);
        });

        builder.Services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();
        builder.Services.AddTransient<ReportRequestedIntegrationEventHandler>();
        builder.Services.AddTransient<ReportCompletedIntegrationEventHandler>();

        builder.Services.AddSingleton<IRabbitMqPersistentConnection>(sp =>
        {
            var factory = new ConnectionFactory
            {
                HostName = configuration["EventBusConnection"],
                DispatchConsumersAsync = true
            };

            if (!string.IsNullOrEmpty(configuration["EventBusUserName"]))
            {
                factory.UserName = configuration["EventBusUserName"];
            }

            if (!string.IsNullOrEmpty(configuration["EventBusPassword"]))
            {
                factory.Password = configuration["EventBusPassword"];
            }

            var retryCount = 5;
            if (!string.IsNullOrEmpty(configuration["EventBusRetryCount"]))
            {
                retryCount = int.Parse(configuration["EventBusRetryCount"]);
            }

            return new DefaultRabbitMqPersistentConnection(factory, retryCount);
        });

        #endregion

        builder.Services.AddTransient<IReportingService, ReportingService>();

        builder.Services.AddOptions();

        //builder.Host.ConfigureContainer<ContainerBuilder>(builder => builder.Register);

        var app = builder.Build();

        // The following switch is used to fix InvalidCastException caused by breaking changes in PostgreSQL's new timestamp behavior.
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        #region Apply Any Pending Migrations

        /* The following code can be used to apply migrations on runtime. Disabled to prevent database corruption.
        using (var scope = app.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ReportingContext>();
            dbContext.Database.Migrate();
        }
        */

        #endregion

        #region Subscribe to Events

        using (var scope = app.Services.CreateScope())
        {
            var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
            eventBus.Subscribe<ReportRequestedIntegrationEvent, ReportRequestedIntegrationEventHandler>();
            eventBus.Subscribe<ReportCompletedIntegrationEvent, ReportCompletedIntegrationEventHandler>();
        }

        #endregion

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static IConfiguration GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddEnvironmentVariables();

        return builder.Build();
    }
}