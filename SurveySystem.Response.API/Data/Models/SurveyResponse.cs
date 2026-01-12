namespace SurveySystem.Response.API.Data.Models;

public class SurveyResponse
{
    public Guid Id { get; set; }
    public Guid SurveyId { get; set; }
    public DateTime SubmittedAt { get; set; }

    public ICollection<ResponseAnswer> Answers { get; set; } = [];
}