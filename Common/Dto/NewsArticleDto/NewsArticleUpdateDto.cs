using System.ComponentModel.DataAnnotations;

namespace Common.Dto.NewsArticleDto;

public class NewsArticleUpdateDto : NewsArticleBaseDto
{
    [Required(ErrorMessage = "ID bài viết là bắt buộc")]
    public string NewsArticleId { get; set; }
    public short CurrentUserId { get; set; }
}