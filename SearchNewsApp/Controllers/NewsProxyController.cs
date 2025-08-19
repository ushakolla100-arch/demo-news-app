using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Text.Json;
using System.Globalization;

[ApiController]
[Route("api/[controller]")]
public class NewsProxyController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _guardianApiKey;

    public NewsProxyController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _guardianApiKey = configuration["GuardianApi:ApiKey"];
    }

    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return BadRequest("Query cannot be empty");

        var client = _httpClientFactory.CreateClient();
        var url = $"https://content.guardianapis.com/search?q={Uri.EscapeDataString(query)}&api-key={_guardianApiKey}&show-fields=trailText,short-url";

        var response = await client.GetStringAsync(url);

        using var jsonDoc = JsonDocument.Parse(response);

        // Materialize array immediately to avoid disposed object
        var resultsArray = jsonDoc.RootElement
                                  .GetProperty("response")
                                  .GetProperty("results")
                                  .EnumerateArray()
                                  .ToList();

        var results = resultsArray.Select(item =>
        {
            // Safe description extraction
            string description = "";
            if (item.TryGetProperty("fields", out var fields) &&
                fields.ValueKind != JsonValueKind.Null &&
                fields.TryGetProperty("trailText", out var trailText) &&
                trailText.ValueKind != JsonValueKind.Null)
            {
                description = trailText.GetString() ?? "";
            }

            // Safe date extraction
            string publishedAt = "";
            if (item.TryGetProperty("webPublicationDate", out var pubDate) &&
                pubDate.ValueKind != JsonValueKind.Null)
            {
                if (DateTime.TryParse(pubDate.GetString(), out var date))
                {
                    publishedAt = date.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                }
            }

            return new
            {
                Id = item.GetProperty("webUrl").GetString(),
                Title = item.GetProperty("webTitle").GetString(),
                Url = item.GetProperty("webUrl").GetString(),
                Description = description,
                PublishedAt = publishedAt
            };
        }).ToList();

        return Ok(results);
    }
}
