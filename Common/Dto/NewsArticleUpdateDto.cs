using System.ComponentModel.DataAnnotations;

namespace Common.Dto;

public class NewsArticleUpdateDto : NewsArticleBaseDto
{
    [Required(ErrorMessage = "ID bài viết là bắt buộc")]
    public string NewsArticleId { get; set; }
}