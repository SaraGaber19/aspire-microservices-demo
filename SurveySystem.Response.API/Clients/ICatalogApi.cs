using Refit;

namespace SurveySystem.Response.API.Clients;

public interface ICatalogApi
{
    // Calls Catalog API GET /surveys/{id}
    [Get("/surveys/{id}")]
    Task<HttpResponseMessage> GetSurveyAsync(
        [AliasAs("id")] Guid id,
        CancellationToken cancellationToken = default);
}