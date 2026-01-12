namespace SurveySystem.Catalog.Domain.Entities;

public class SurveyingApplication
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public List<Survey> Surveys { get; set; } = [];
}