namespace SearchNewsApp.Models
{
    public class NewsArticle
    {
        public string Id { get; set; }           
        public string Title { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public string PublishedAt { get; set; }  
    }

    public class NewsViewModel
    {
        public string Query { get; set; }
        public List<NewsArticle> SearchResults { get; set; } = new();
        public List<NewsArticle> PinnedArticles { get; set; } = new();
    }

}
