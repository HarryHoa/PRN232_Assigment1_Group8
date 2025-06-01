using System.Text.Json.Serialization;

namespace Common.Dto;

public class NewArticleRes
{
    [JsonPropertyName("totalPosts")]
    public int TotalPosts { get; set; }

    [JsonPropertyName("averagePerDay")]
    public int AveragePerDay  { get; set; }

    [JsonPropertyName("postsMonth")]
    public int PostsMonth { get; set; }

    [JsonPropertyName("postsToday")]
    public int PostsToday { get; set; }

    [JsonPropertyName("listArticle")]
    public List<ListNewArticleRes> listArticle { get; set; }

}