namespace Common.Dto;

public class NewArticleRes
{
    public string NewsTitle { get; set; }
    public string Headline { get; set; }
    public string NewsContent { get; set; }
    public string NewsSource { get; set; }
    public string CategoryName { get; set; }    
    public DateTime CreatedDate { get; set; }
}