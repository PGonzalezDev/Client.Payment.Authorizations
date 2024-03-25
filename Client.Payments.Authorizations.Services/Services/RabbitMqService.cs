using Client.Payments.Authorizations.Services.Helper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Client.Payments.Authorizations.Services;

public class RabbitMqService : IRabbitMqService
{
    private readonly ILogger<RabbitMqService> _logger;

    private readonly string _host;
    private readonly string _exchangeName;

    public RabbitMqService(
        ILogger<RabbitMqService> logger,
        IConfiguration config
    )
    {
        _logger = logger;

        _host = config["rmqHost"] ?? "localhost";
        _exchangeName = config["rmqExchange"] ?? "approved-auth-exchange";
    }

    public void Publish<T>(T value) where T : class
    {
        using IModel channel = BuildChannel();

        _logger.LogInformation($"json approved Auth: {JsonSerializer.Serialize(value)}");

        channel.BasicPublish(
            exchange: _exchangeName,
            routingKey: string.Empty,
            basicProperties: null,
            body: RabbitMqMessageEncoderHelper.EncodeMessage<T>(value)
        );
    }

    public IModel BuildChannel()
    {
        ConnectionFactory factory = new() { HostName = _host };
        IConnection connection = factory.CreateConnection();
        IModel channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: _exchangeName, type: ExchangeType.Fanout);

        return channel;
    }

    public string QueueBindToExchange(IModel channel)
    {
        var queueName = channel.QueueDeclare().QueueName;
        channel.QueueBind(
            queue: queueName,
            exchange: _exchangeName,
            routingKey: string.Empty
        );

        return queueName;
    }

    public EventingBasicConsumer BuildConsumer(IModel channel, EventHandler<BasicDeliverEventArgs> handler)
    {
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += handler;

        return consumer;
    }
}
