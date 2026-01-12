namespace SurveySystem.Catalog.Application.DTOs;

public record SurveyDto(Guid Id, Guid ApplicationId, string Title, string? Description);
public record CreateSurveyDto(Guid ApplicationId, string Title, string? Description);
public record UpdateSurveyDto(string Title, string? Description);

public record QuestionDto(Guid Id, Guid SurveyId, string Text, int Order, List<QuestionOptionDto> Options);
public record CreateQuestionDto(Guid SurveyId, string Text, int Order, List<CreateQuestionOptionDto> Options);
public record UpdateQuestionDto(string Text, int Order, List<UpdateQuestionOptionDto> Options);

public record QuestionOptionDto(Guid Id, int Code, string Value);
public record CreateQuestionOptionDto(int Code, string Value);
public record UpdateQuestionOptionDto(int Code, string Value);
