using DAL.Models;

namespace DAL.Repository.Impl;

public class NewArticleRepository : GenericRepository<NewsArticle>
{
    private readonly FUNewsManagementContext _context;

    public NewArticleRepository(FUNewsManagementContext context) : base(context)
    {
        _context = context;
    }
}