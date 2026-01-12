using Microsoft.EntityFrameworkCore;
using SurveySystem.Catalog.Domain.Entities;

namespace SurveySystem.Catalog.Infrastructure.Data;

public class CatalogContext(DbContextOptions<CatalogContext> options)
    : DbContext(options)
{
    public DbSet<SurveyingApplication> Applications => Set<SurveyingApplication>();
    public DbSet<Survey> Surveys => Set<Survey>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<QuestionOption> QuestionOptions => Set<QuestionOption>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Application
        modelBuilder.Entity<SurveyingApplication>(b =>
        {
            b.HasKey(a => a.Id);
            b.Property(a => a.Name).IsRequired().HasMaxLength(200);
            b.Property(a => a.Description).HasMaxLength(1000);
            b.HasMany(a => a.Surveys)
                .WithOne(s => s.Application!)
                .HasForeignKey(s => s.ApplicationId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Survey
        modelBuilder.Entity<Survey>(b =>
        {
            b.HasKey(s => s.Id);
            b.Property(s => s.Title).IsRequired().HasMaxLength(200);
            b.Property(s => s.Description).HasMaxLength(1000);
            b.HasMany(s => s.Questions)
                .WithOne(q => q.Survey!)
                .HasForeignKey(q => q.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Question
        modelBuilder.Entity<Question>(b =>
        {
            b.HasKey(q => q.Id);
            b.Property(q => q.Text).IsRequired().HasMaxLength(1000);
            b.Property(q => q.Order).IsRequired();
            b.HasIndex(q => new { q.SurveyId, q.Order }).IsUnique();
            b.HasMany(q => q.Options)
                .WithOne(o => o.Question!)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // QuestionOption
        modelBuilder.Entity<QuestionOption>(b =>
        {
            b.HasKey(o => o.Id);
            b.Property(o => o.Code).IsRequired();
            b.Property(o => o.Value).IsRequired().HasMaxLength(500);
            b.HasIndex(o => new { o.QuestionId, o.Code }).IsUnique();
        });
    }
}