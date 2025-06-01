using System.Text.Json.Serialization;

namespace Common.Dto;

public class ListNewArticleRes
{
    [JsonPropertyName("newsTitle")]
    public string NewsTitle { get; set; }

    [JsonPropertyName("headline")]
    public string Headline { get; set; }

    [JsonPropertyName("newsContent")]
    public string NewsContent { get; set; }

    [JsonPropertyName("newsSource")]
    public string NewsSource { get; set; }

    [JsonPropertyName("categoryName")]
    public string CategoryName { get; set; }

    [JsonPropertyName("createdDate")]
    public DateTime CreatedDate { get; set; }
}