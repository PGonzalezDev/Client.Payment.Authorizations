using Client.Payments.Authorizations.Context;
using Client.Payments.Authorizations.Models;
using Client.Payments.Authorizations.Services.Helper;
using Client.Payments.Authorizations.Services.Repositories;
using Client.Payments.Authorizations.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using System.Text.Json;


var configBuilder = new ConfigurationBuilder().AddJsonFile($"appsettings.json");
var configuration = configBuilder.Build();

var loggerFactory = LoggerFactory.Create(
    builder => builder.SetMinimumLevel(LogLevel.Debug)
);

var logger = loggerFactory.CreateLogger<RabbitMqService>();

var serviceProvider = new ServiceCollection()
    .AddDbContext<ClientPaymentAuthorizationContext>(opt =>
        opt.UseInMemoryDatabase("ClientPaymentAuthorizations")
    )
    .AddScoped<IApprovedAuthorizationRepository, ApprovedAuthorizationRepository>()
    .AddScoped<IRabbitMqService, RabbitMqService>()
    .AddSingleton(logger)
    .AddSingleton<IConfiguration>(configuration)
    .BuildServiceProvider();

var rmqService = serviceProvider.GetService<IRabbitMqService>();

if (rmqService == null) { throw new NullReferenceException(nameof(RabbitMqService)); }

var channel = rmqService.BuildChannel();
string queueName = rmqService.QueueBindToExchange(channel);

Console.WriteLine(" [*] Waiting for logs.");

var consumer = rmqService.BuildConsumer(
    channel,
    (model, ea) =>
    {
        byte[] body = ea.Body.ToArray();
        var approvedAuth = RabbitMqMessageEncoderHelper.DecodeMessage<ApprovedAuthorization>(body);

        Console.WriteLine($" [x] {JsonSerializer.Serialize(approvedAuth)}");

        var repository = serviceProvider.GetService<IApprovedAuthorizationRepository>();
        repository.Add(approvedAuth);
    }
);

channel.BasicConsume(
    queue: queueName,
    autoAck: true,
    consumer: consumer
);

Console.WriteLine(" Press [enter] to exit.");
Console.ReadLine();