using SurveySystem.Catalog.Application.DTOs;
using SurveySystem.Catalog.Application.Mappings;
using SurveySystem.Catalog.Application.Repositories;
using SurveySystem.Catalog.Domain.Entities;

namespace SurveySystem.Catalog.Application.Services;

public class SurveyService(ISurveyRepository surveys)
{
    public Task<Survey?> GetAsync(Guid id, CancellationToken ct) => surveys.GetAsync(id, ct);
    public Task<List<Survey>> GetAllAsync(CancellationToken ct) => surveys.GetAllAsync(ct);
    public async Task<Survey> CreateAsync(CreateSurveyDto dto, CancellationToken ct)
    {
        var entity = dto.ToEntity();
        await surveys.AddAsync(entity, ct);
        return entity;
    }
    public async Task<Survey?> UpdateAsync(Guid id, UpdateSurveyDto dto, CancellationToken ct)
    {
        var entity = await surveys.GetAsync(id, ct);
        if (entity is null) return null;
        entity.UpdateFrom(dto);
        await surveys.UpdateAsync(entity, ct);
        return entity;
    }
    public Task DeleteAsync(Guid id, CancellationToken ct) => surveys.DeleteAsync(id, ct);
}

public class QuestionService(IQuestionRepository questions)
{
    public Task<Question?> GetAsync(Guid id, CancellationToken ct) => questions.GetAsync(id, ct);
    public Task<List<Question>> GetBySurveyAsync(Guid surveyId, CancellationToken ct) => questions.GetBySurveyAsync(surveyId, ct);
    public async Task<Question> CreateAsync(CreateQuestionDto dto, CancellationToken ct)
    {
        var entity = dto.ToEntity();
        // fix QuestionId for options
        foreach (var opt in entity.Options) opt.QuestionId = entity.Id;
        await questions.AddAsync(entity, ct);
        return entity;
    }
    public async Task<Question?> UpdateAsync(Guid id, UpdateQuestionDto dto, CancellationToken ct)
    {
        var entity = await questions.GetAsync(id, ct);
        if (entity is null) return null;
        entity.UpdateFrom(dto);
        await questions.UpdateAsync(entity, ct);
        return entity;
    }
    public Task DeleteAsync(Guid id, CancellationToken ct) => questions.DeleteAsync(id, ct);
}
