using Scalar.AspNetCore;
using StackExchange.Redis;
using SurveySystem.Reporting.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddOpenApi();

builder.AddRabbitMQClient("rabbit-mq");

builder.AddRedisClient("reporting-redis-db");

builder.Services.AddHostedService<SubmissionConsumer>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/reports/surveys/{surveyId}/responses/count",
    async (
        Guid surveyId,
        IConnectionMultiplexer redis,
        CancellationToken ct) =>
{
    var db = redis.GetDatabase();
    var key = $"survey:{surveyId}:responses";
    var value = await db.StringGetAsync(key);
    _ = long.TryParse(value.ToString(), out var parsed);
    return Results.Ok(new { surveyId, parsed });
})
.Produces(StatusCodes.Status200OK);


app.Run();