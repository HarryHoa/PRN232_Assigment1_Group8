namespace Common.Dto;

public class NewArticleRes
{
    public int TotalPosts { get; set; }
    public int AveragePerDay  { get; set; }
    public int PostsMonth { get; set; }
    public int PostsToday { get; set; }

    public List<ListNewArticleRes> listArticle { get; set; }

}