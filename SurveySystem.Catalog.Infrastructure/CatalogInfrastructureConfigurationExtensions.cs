using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SurveySystem.Catalog.Application.Repositories;
using SurveySystem.Catalog.Domain.Entities;
using SurveySystem.Catalog.Infrastructure.Data;
using SurveySystem.Catalog.Infrastructure.Repositories;

namespace SurveySystem.Catalog.Infrastructure;

public static class CatalogInfrastructureConfigurationExtensions
{
    public static IHostApplicationBuilder AddInfrastructureServices(
        this IHostApplicationBuilder builder)
    {
        #region Database Context

        var connectionString = builder.Configuration
            .GetConnectionString("catalog-postgres-db");

        builder.Services.AddDbContext<CatalogContext>(
            dbContextOptionsBuilder => dbContextOptionsBuilder
            .UseNpgsql(
                connectionString,
                sqlOptions => sqlOptions.MigrationsAssembly("SurveySystem.Catalog.MigrationService"))
            .UseAsyncSeeding(async (context, _, cancellationToken) =>
                {
                    // Seed Application
                    var appName = "Demo Application";
                    var app = await context.Set<SurveyingApplication>()
                        .FirstOrDefaultAsync(a => a.Name == appName, cancellationToken);
                    if (app is null)
                    {
                        app = new SurveyingApplication
                        {
                            Id = Guid.NewGuid(),
                            Name = appName,
                            Description = "Seeded demo application"
                        };
                        await context.Set<SurveyingApplication>().AddAsync(app, cancellationToken);
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    // Seed Survey
                    var surveyTitle = "Welcome Survey";
                    var survey = await context.Set<Survey>()
                        .FirstOrDefaultAsync(s => s.Title == surveyTitle && s.ApplicationId == app.Id, cancellationToken);
                    if (survey is null)
                    {
                        survey = new Survey
                        {
                            Id = Guid.NewGuid(),
                            ApplicationId = app.Id,
                            Title = surveyTitle,
                            Description = "Basic seeded survey"
                        };
                        await context.Set<Survey>().AddAsync(survey, cancellationToken);
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    // Seed Question
                    var questionText = "How did you hear about us?";
                    var question = await context.Set<Question>()
                        .FirstOrDefaultAsync(q => q.SurveyId == survey.Id && q.Text == questionText, cancellationToken);
                    if (question is null)
                    {
                        question = new Question
                        {
                            Id = Guid.NewGuid(),
                            SurveyId = survey.Id,
                            Text = questionText,
                            Order = 1,
                            Options = new List<QuestionOption>()
                        };
                        await context.Set<Question>().AddAsync(question, cancellationToken);
                        await context.SaveChangesAsync(cancellationToken);
                    }

                    // Seed Question Options
                    var existingOptions = await context.Set<QuestionOption>()
                        .Where(o => o.QuestionId == question.Id)
                        .ToListAsync(cancellationToken);
                    if (existingOptions.Count == 0)
                    {
                        var options = new[]
                        {
                            new QuestionOption { Id = Guid.NewGuid(), QuestionId = question.Id, Code = 1, Value = "Search Engine" },
                            new QuestionOption { Id = Guid.NewGuid(), QuestionId = question.Id, Code = 2, Value = "Social Media" },
                            new QuestionOption { Id = Guid.NewGuid(), QuestionId = question.Id, Code = 3, Value = "Friend/Referral" },
                            new QuestionOption { Id = Guid.NewGuid(), QuestionId = question.Id, Code = 4, Value = "Other" }
                        };
                        await context.Set<QuestionOption>().AddRangeAsync(options, cancellationToken);
                        await context.SaveChangesAsync(cancellationToken);
                    }
                }));

        builder.EnrichNpgsqlDbContext<CatalogContext>();

        #endregion

        #region Repositories

        builder.Services.AddScoped<ISurveyRepository, SurveyRepository>();
        builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();

        #endregion

        return builder;
    }
}
