using RabbitMQ.Client;
using SurveySystem.Response.API.Data.Models;
using System.Text;
using System.Text.Json;

namespace SurveySystem.Response.API.Services;

public class SubmissionPublisher(IConnection connection)
{
    public async Task PublishSubmittedAsync(SurveyResponse response, CancellationToken ct = default)
    {
        using var channel = await connection.CreateChannelAsync();

        const string queueName = "sample-submitted";
        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var payload = new
        {
            ResponseId = response.Id,
            SurveyId = response.SurveyId,
            SubmittedAt = response.SubmittedAt,
            Answers = response.Answers.Select(a => new
            {
                a.QuestionId,
                a.QuestionOptionId
            })
        };

        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(payload));
        var props = new BasicProperties
        {
            ContentType = "application/json",
            DeliveryMode = DeliveryModes.Persistent
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            mandatory: false,
            basicProperties: props,
            body: body,
            cancellationToken: ct);
    }
}