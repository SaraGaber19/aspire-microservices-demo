namespace SurveySystem.Catalog.Domain.Entities;

public class Question
{
    public Guid Id { get; set; }
    public Guid SurveyId { get; set; }
    public string Text { get; set; } = string.Empty;
    public int Order { get; set; }
    public required List<QuestionOption> Options { get; set; }
    public Survey? Survey { get; set; }
}