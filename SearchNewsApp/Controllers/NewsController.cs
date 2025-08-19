using Microsoft.AspNetCore.Mvc;
using SearchNewsApp.Models;
using System.Globalization;
using System.Net.Http;
using System.Text.Json;
public class NewsController : Controller
{
    private readonly IHttpClientFactory _httpClientFactory;

    public NewsController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<IActionResult> Index(string query)
    {
        var model = new NewsViewModel { Query = query };

        if (!string.IsNullOrWhiteSpace(query))
        {
            var client = _httpClientFactory.CreateClient();
            // Call your own proxy API instead of Guardian API
            var apiUrl = $"https://localhost:7123/api/NewsProxy/search?query={Uri.EscapeDataString(query)}";

            var response = await client.GetStringAsync(apiUrl);
            var articles = JsonSerializer.Deserialize<List<NewsArticle>>(response, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (articles != null)
                model.SearchResults = articles;
        }

        return View(model);
    }
}
