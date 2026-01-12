using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace SurveySystem.Reporting.API.Services;

public class SubmissionConsumer(
    IConnection rabbitConnection,
    IConnectionMultiplexer redis) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var channel = await rabbitConnection.CreateChannelAsync(cancellationToken: stoppingToken);
        const string queueName = "sample-submitted";
        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var db = redis.GetDatabase();

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (_, ea) =>
        {
            try
            {
                var json = Encoding.UTF8.GetString(ea.Body.ToArray());
                var message = JsonSerializer.Deserialize<SubmittedMessage>(json);
                if (message is not null)
                {
                    var key = $"survey:{message.SurveyId}:responses";
                    await db.StringIncrementAsync(key);
                }

                await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch
            {
                await channel.BasicNackAsync(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true);
            }
        };

        await channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);

        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private record SubmittedMessage(
        Guid Id,
        Guid SurveyId,
        DateTime SubmittedAt);
}
