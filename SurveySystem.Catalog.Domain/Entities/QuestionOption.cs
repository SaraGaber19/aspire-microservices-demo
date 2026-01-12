namespace SurveySystem.Catalog.Domain.Entities;

public class QuestionOption
{
    public Guid Id { get; set; }
    public Guid QuestionId { get; set; }
    public int Code { get; set; }
    public string Value { get; set; } = string.Empty;
    public Question? Question { get; set; }
}