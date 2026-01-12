using MongoDB.Driver;
using Refit;
using Scalar.AspNetCore;
using SurveySystem.Response.API.Clients;
using SurveySystem.Response.API.Data.Context;
using SurveySystem.Response.API.Data.Models;
using SurveySystem.Response.API.DTOs;
using SurveySystem.Response.API.Services;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

var dbConfigName = "response-mongo-db";

builder.AddServiceDefaults();
builder.Services.AddOpenApi();

builder.AddMongoDBClient(dbConfigName);

builder.Services
    .AddRefitClient<ICatalogApi>()
    .ConfigureHttpClient(static (c) =>
    {
        c.BaseAddress = new Uri("https+http://catalog-api");
    })
    .AddStandardResilienceHandler();

builder.AddRabbitMQClient("rabbit-mq");

builder.Services.AddSingleton<SubmissionPublisher>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapPost("/responses", async (
    CreateResponseDto dto,
    ICatalogApi catalog,
    IMongoDatabase mongoDb,
    SubmissionPublisher publisher,
    CancellationToken ct) =>
{
    var dbContext = ResponseContext.Create(mongoDb);
    HttpResponseMessage catalogResp;
    try
    {
        catalogResp = await catalog.GetSurveyAsync(dto.SurveyId, ct);
    }
    catch (ApiException apiEx) when (apiEx.StatusCode == HttpStatusCode.NotFound)
    {
        return Results.NotFound(new { error = "Survey not found" });
    }
    catch
    {
        return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
    }

    if (!catalogResp.IsSuccessStatusCode)
    {
        return catalogResp.StatusCode == HttpStatusCode.NotFound
            ? Results.NotFound(new { error = "Survey not found" })
            : Results.BadRequest(new { error = "Unable to validate survey" });
    }

    var newId = Guid.NewGuid();
    var entity = new SurveyResponse
    {
        Id = newId,
        SurveyId = dto.SurveyId,
        SubmittedAt = DateTime.UtcNow,
        Answers = dto.Answers.Select(a => new ResponseAnswer
        {
            Id = Guid.NewGuid(),
            QuestionId = a.QuestionId,
            QuestionOptionId = a.QuestionOptionId
        }).ToList()
    };

    await dbContext.Responses.AddAsync(entity, ct);
    await dbContext.SaveChangesAsync(ct);

    await publisher.PublishSubmittedAsync(entity, ct);

    return Results.Created(
        $"/responses/{entity.Id}",
        new
        {
            id = entity.Id,
            entity.SurveyId,
            createdAt = entity.SubmittedAt
        });
})
.Produces(StatusCodes.Status201Created)
.Produces(StatusCodes.Status404NotFound)
.Produces(StatusCodes.Status400BadRequest);

app.Run();