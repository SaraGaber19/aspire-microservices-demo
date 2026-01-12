namespace SurveySystem.Response.API.DTOs;

public record ResponseAnswerDto(
    Guid QuestionId,
    Guid QuestionOptionId);