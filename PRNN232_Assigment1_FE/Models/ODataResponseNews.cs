using System.Text.Json.Serialization;

namespace PRNN232_Assigment1_FE.Models
{
    public class ODataResponseNews<T>
    {
        [JsonPropertyName("@odata.context")]
        public string? Context { get; set; }

        [JsonPropertyName("@odata.count")]
        public int Count { get; set; }

        [JsonPropertyName("@odata.nextLink")]
        public string? NextLink { get; set; }

        [JsonPropertyName("value")]
        public T Value { get; set; } = default(T)!;
    }

    // ✅ ADD: Specific model for NewsArticle OData response
    public class NewsArticleODataResponse
    {
        [JsonPropertyName("@odata.context")]
        public string? Context { get; set; }

        [JsonPropertyName("@odata.count")]
        public int Count { get; set; }

        [JsonPropertyName("@odata.nextLink")]
        public string? NextLink { get; set; }

        [JsonPropertyName("value")]
        public List<NewsArticleODataItem> Value { get; set; } = new();
    }

    // ✅ ADD: Model that matches the actual OData entity structure
    public class NewsArticleODataItem
    {
        [JsonPropertyName("NewsArticleId")]
        public string NewsArticleId { get; set; } = string.Empty;

        [JsonPropertyName("NewsTitle")]
        public string NewsTitle { get; set; } = string.Empty;

        [JsonPropertyName("Headline")]
        public string Headline { get; set; } = string.Empty;

        [JsonPropertyName("CreatedDate")]
        public DateTime? CreatedDate { get; set; }

        [JsonPropertyName("NewsContent")]
        public string NewsContent { get; set; } = string.Empty;

        [JsonPropertyName("NewsSource")]
        public string? NewsSource { get; set; }

        [JsonPropertyName("CategoryId")]
        public short? CategoryId { get; set; }

        [JsonPropertyName("NewsStatus")]
        public bool? NewsStatus { get; set; }

        [JsonPropertyName("CreatedById")]
        public short? CreatedById { get; set; }

        [JsonPropertyName("UpdatedById")]
        public short? UpdatedById { get; set; }

        [JsonPropertyName("ModifiedDate")]
        public DateTime? ModifiedDate { get; set; }

        // Related entities (when $expand is used)
        [JsonPropertyName("Category")]
        public CategoryODataItem? Category { get; set; }

        [JsonPropertyName("CreatedBy")]
        public SystemAccountODataItem? CreatedBy { get; set; }

        [JsonPropertyName("Tags")]
        public List<TagODataItem> Tags { get; set; } = new();
    }

    public class CategoryODataItem
    {
        [JsonPropertyName("CategoryId")]
        public short CategoryId { get; set; }

        [JsonPropertyName("CategoryName")]
        public string CategoryName { get; set; } = string.Empty;

        [JsonPropertyName("CategoryDesciption")]
        public string CategoryDesciption { get; set; } = string.Empty;
    }

    public class SystemAccountODataItem
    {
        [JsonPropertyName("AccountId")]
        public short AccountId { get; set; }

        [JsonPropertyName("AccountName")]
        public string AccountName { get; set; } = string.Empty;

        [JsonPropertyName("AccountEmail")]
        public string AccountEmail { get; set; } = string.Empty;
    }

    public class TagODataItem
    {
        [JsonPropertyName("TagId")]
        public int TagId { get; set; }

        [JsonPropertyName("TagName")]
        public string TagName { get; set; } = string.Empty;

        [JsonPropertyName("Note")]
        public string? Note { get; set; }
    }
}