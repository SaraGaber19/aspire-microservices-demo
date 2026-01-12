namespace SurveySystem.Catalog.Domain.Entities;

public class Survey
{
    public Guid Id { get; set; }
    public Guid ApplicationId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public SurveyingApplication? Application { get; set; }
    public List<Question> Questions { get; set; } = [];
}