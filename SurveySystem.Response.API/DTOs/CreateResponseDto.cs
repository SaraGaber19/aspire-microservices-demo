namespace SurveySystem.Response.API.DTOs;

public record CreateResponseDto(
    Guid SurveyId,
    Guid? RespondentId,
    IEnumerable<ResponseAnswerDto> Answers);