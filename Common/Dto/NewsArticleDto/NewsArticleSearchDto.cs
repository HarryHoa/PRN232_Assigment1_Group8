namespace Common.Dto;

public class NewsArticleSearchDto
{
    public string? SearchTerm { get; set; }
    public short? CategoryId { get; set; }
    public List<int>? TagIds { get; set; }
    public bool? NewsStatus { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; } = "CreatedDate";
    public string? SortOrder { get; set; } = "desc";
}
