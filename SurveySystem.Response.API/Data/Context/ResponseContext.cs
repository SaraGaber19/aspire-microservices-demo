using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using SurveySystem.Response.API.Data.Models;

namespace SurveySystem.Response.API.Data.Context;

public class ResponseContext(DbContextOptions<ResponseContext> options)
    : DbContext(options)
{
    public static ResponseContext Create(IMongoDatabase database) =>
        new(new DbContextOptionsBuilder<ResponseContext>()
            .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
            .Options);

    public DbSet<SurveyResponse> Responses { get; set; }
}
