using Scalar.AspNetCore;
using SurveySystem.Catalog.Application.DTOs;
using SurveySystem.Catalog.Application.Mappings;
using SurveySystem.Catalog.Application.Services;
using SurveySystem.Catalog.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.AddInfrastructureServices();
builder.Services.AddOpenApi();

builder.Services.AddScoped<SurveyService>();
builder.Services.AddScoped<QuestionService>();

var app = builder.Build();

app.MapDefaultEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.MapGet("/surveys", async (SurveyService svc, CancellationToken ct)
    => (await svc.GetAllAsync(ct)).Select(s => s.ToDto()));

app.MapGet("/surveys/{id}", async (Guid id, SurveyService svc, CancellationToken ct)
    =>
{
    var survey = await svc.GetAsync(id, ct);
    return survey is not null ? Results.Ok(survey.ToDto()) : Results.NotFound();
}
    )
    .WithName("GetSurvey")
    .Produces<SurveyDto>()
    .Produces(StatusCodes.Status404NotFound);

app.MapPost("/surveys", async (CreateSurveyDto dto, SurveyService svc, CancellationToken ct)
    => Results.CreatedAtRoute("GetSurvey", new { id = (await svc.CreateAsync(dto, ct)).Id }));

app.MapPut("/surveys/{id}", async (Guid id, UpdateSurveyDto dto, SurveyService svc, CancellationToken ct)
    => (await svc.UpdateAsync(id, dto, ct)) is null ? Results.NotFound() : Results.NoContent());

app.MapDelete("/surveys/{id}", async (Guid id, SurveyService svc, CancellationToken ct)
    =>
{ await svc.DeleteAsync(id, ct); return Results.NoContent(); })
    .Produces(StatusCodes.Status204NoContent);

app.MapGet("/surveys/{surveyId}/questions", async (Guid surveyId, QuestionService svc, CancellationToken ct)
    => (await svc.GetBySurveyAsync(surveyId, ct)).Select(q => q.ToDto()));

app.MapGet("/questions/{id}", async (Guid id, QuestionService svc, CancellationToken ct)
    => (await svc.GetAsync(id, ct))?.ToDto())
    .WithName("GetQuestion")
    .Produces<QuestionDto>();

app.MapPost("/questions", async (CreateQuestionDto dto, QuestionService svc, CancellationToken ct)
    => Results.CreatedAtRoute("GetQuestion", new { id = (await svc.CreateAsync(dto, ct)).Id }));

app.MapPut("/questions/{id}", async (Guid id, UpdateQuestionDto dto, QuestionService svc, CancellationToken ct)
    => (await svc.UpdateAsync(id, dto, ct)) is null ? Results.NotFound() : Results.NoContent());

app.MapDelete("/questions/{id}", async (Guid id, QuestionService svc, CancellationToken ct)
    =>
{
    await svc.DeleteAsync(id, ct);
    return Results.NoContent();
})
    .Produces(StatusCodes.Status204NoContent);

app.Run();