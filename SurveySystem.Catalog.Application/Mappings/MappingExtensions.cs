using SurveySystem.Catalog.Application.DTOs;
using SurveySystem.Catalog.Domain.Entities;

namespace SurveySystem.Catalog.Application.Mappings;

public static class MappingExtensions
{
    public static SurveyDto ToDto(this Survey s)
        => new(s.Id, s.ApplicationId, s.Title, s.Description);

    public static QuestionDto ToDto(this Question q)
        => new(q.Id, q.SurveyId, q.Text, q.Order, q.Options.Select(o => o.ToDto()).ToList());

    public static QuestionOptionDto ToDto(this QuestionOption o)
        => new(o.Id, o.Code, o.Value);

    public static Survey ToEntity(this CreateSurveyDto dto)
        => new() { Id = Guid.NewGuid(), ApplicationId = dto.ApplicationId, Title = dto.Title, Description = dto.Description };

    public static void UpdateFrom(this Survey s, UpdateSurveyDto dto)
    {
        s.Title = dto.Title;
        s.Description = dto.Description;
    }

    public static Question ToEntity(this CreateQuestionDto dto)
        => new()
        {
            Id = Guid.NewGuid(),
            SurveyId = dto.SurveyId,
            Text = dto.Text,
            Order = dto.Order,
            Options = dto.Options.Select(o => new QuestionOption
            {
                Id = Guid.NewGuid(),
                QuestionId = Guid.Empty, // set after question has an Id
                Code = o.Code,
                Value = o.Value,
            }).ToList()
        };

    public static void UpdateFrom(this Question q, UpdateQuestionDto dto)
    {
        q.Text = dto.Text;
        q.Order = dto.Order;
        // naive options update: replace all
        q.Options = dto.Options.Select(o => new QuestionOption
        {
            Id = Guid.NewGuid(),
            QuestionId = q.Id,
            Code = o.Code,
            Value = o.Value,
        }).ToList();
    }
}
