using System.ComponentModel.DataAnnotations;

namespace Common.Dto;

public abstract class NewsArticleBaseDto
{
    [Required(ErrorMessage = "Tiêu đề bài viết là bắt buộc")]
    [StringLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Tiêu đề phụ là bắt buộc")]
    [StringLength(300, ErrorMessage = "Tiêu đề phụ không được vượt quá 300 ký tự")]
    public string Headline { get; set; }

    [Required(ErrorMessage = "Nội dung bài viết là bắt buộc")]
    public string Content { get; set; }

    [StringLength(100, ErrorMessage = "Nguồn tin không được vượt quá 100 ký tự")]
    public string? Source { get; set; }

    [Required(ErrorMessage = "Danh mục là bắt buộc")]
    public short CategoryId { get; set; }

    public bool Status { get; set; } = true;

    public List<int> TagIds { get; set; } = new List<int>();
    public List<TagDto> Tags { get; set; } = new List<TagDto>();
}