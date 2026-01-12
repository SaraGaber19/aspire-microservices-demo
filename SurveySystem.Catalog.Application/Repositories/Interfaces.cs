namespace SurveySystem.Catalog.Application.Repositories;

using SurveySystem.Catalog.Domain.Entities;

public interface ISurveyRepository
{
    Task<Survey?> GetAsync(Guid id, CancellationToken ct);
    Task<List<Survey>> GetAllAsync(CancellationToken ct);
    Task AddAsync(Survey survey, CancellationToken ct);
    Task UpdateAsync(Survey survey, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}

public interface IQuestionRepository
{
    Task<Question?> GetAsync(Guid id, CancellationToken ct);
    Task<List<Question>> GetBySurveyAsync(Guid surveyId, CancellationToken ct);
    Task AddAsync(Question question, CancellationToken ct);
    Task UpdateAsync(Question question, CancellationToken ct);
    Task DeleteAsync(Guid id, CancellationToken ct);
}
