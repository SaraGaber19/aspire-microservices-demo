using Microsoft.EntityFrameworkCore;
using SurveySystem.Catalog.Application.Repositories;
using SurveySystem.Catalog.Domain.Entities;
using SurveySystem.Catalog.Infrastructure.Data;

namespace SurveySystem.Catalog.Infrastructure.Repositories;

public class SurveyRepository(CatalogContext db) : ISurveyRepository
{
    public Task<Survey?> GetAsync(Guid id, CancellationToken ct)
        => db.Surveys.Include(s => s.Questions).ThenInclude(q => q.Options).FirstOrDefaultAsync(s => s.Id == id, ct);

    public Task<List<Survey>> GetAllAsync(CancellationToken ct)
        => db.Surveys.AsNoTracking().ToListAsync(ct);

    public async Task AddAsync(Survey survey, CancellationToken ct)
    {
        await db.Surveys.AddAsync(survey, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Survey survey, CancellationToken ct)
    {
        db.Surveys.Update(survey);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await db.Surveys.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (entity is null) return;
        db.Surveys.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}

public class QuestionRepository(CatalogContext db) : IQuestionRepository
{
    public Task<Question?> GetAsync(Guid id, CancellationToken ct)
        => db.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == id, ct);

    public Task<List<Question>> GetBySurveyAsync(Guid surveyId, CancellationToken ct)
        => db.Questions.Where(q => q.SurveyId == surveyId).Include(q => q.Options).ToListAsync(ct);

    public async Task AddAsync(Question question, CancellationToken ct)
    {
        await db.Questions.AddAsync(question, ct);
        await db.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(Question question, CancellationToken ct)
    {
        db.Questions.Update(question);
        await db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(Guid id, CancellationToken ct)
    {
        var entity = await db.Questions.FirstOrDefaultAsync(q => q.Id == id, ct);
        if (entity is null) return;
        db.Questions.Remove(entity);
        await db.SaveChangesAsync(ct);
    }
}
