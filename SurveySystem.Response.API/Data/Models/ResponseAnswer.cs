namespace SurveySystem.Response.API.Data.Models;

public class ResponseAnswer
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public Guid QuestionOptionId { get; set; }
}
