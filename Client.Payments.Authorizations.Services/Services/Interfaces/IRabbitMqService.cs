using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Client.Payments.Authorizations.Services;

public interface IRabbitMqService
{
    void Publish<T>(T value) where T : class;
    IModel BuildChannel();
    string QueueBindToExchange(IModel channel);
    EventingBasicConsumer BuildConsumer(IModel channel, EventHandler<BasicDeliverEventArgs> handler);
}
