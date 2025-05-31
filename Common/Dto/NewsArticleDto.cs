

namespace Common.Dto;
public class NewsArticleDto : NewsArticleBaseDto
{
    public string? NewsArticleId { get; set; }

    // Read-only properties for display
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string? CategoryName { get; set; }
    public List<TagDto> Tags { get; set; } = new List<TagDto>();
    public string? CreatedByName { get; set; }
}