using DAL.Models;
using DLL.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace PRN232_ASSI1.Controllers
{
    [Route("odata/[controller]")]
    public class NewsArticleODataController : ODataController
    {
        private readonly FUNewsManagementContext _context;
        private readonly INewArticleService _newArticleService;

        public NewsArticleODataController(FUNewsManagementContext context, INewArticleService newArticleService)
        {
            _context = context;
            _newArticleService = newArticleService;
        }

        /// <summary>
        /// GET: odata/NewsArticleOData
        /// Supports: $filter, $orderby, $top, $skip, $count, $select, $expand
        /// Examples:
        /// - odata/NewsArticleOData?$filter=contains(NewsTitle,'test')
        /// - odata/NewsArticleOData?$filter=NewsStatus eq true
        /// - odata/NewsArticleOData?$orderby=CreatedDate desc
        /// - odata/NewsArticleOData?$expand=Category,CreatedBy,Tags
        /// - odata/NewsArticleOData?$count=true&$top=10
        /// </summary>
        [EnableQuery(PageSize = 20, MaxTop = 100)]
        [HttpGet]
        public IQueryable<NewsArticle> Get()
        {
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .AsQueryable();
        }

        /// <summary>
        /// GET: odata/NewsArticleOData/{key}
        /// Example: odata/NewsArticleOData/articleId123?$expand=Category,Tags
        /// </summary>
        [EnableQuery]
        [HttpGet("{key}")]  // ← FIX: Add explicit route parameter
        public IQueryable<NewsArticle> GetByKey([FromRoute] string key)  // ← FIX: Rename method
        {
            return _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .Where(n => n.NewsArticleId == key);
        }
        
        
        // [EnableQuery]
        [HttpGet("statistics")]
        public async Task<IActionResult> GetAll(ODataQueryOptions<NewsArticle> odataOptions)
        {
            var response = await _newArticleService.GetNewsByODataAsync(odataOptions);
            if (!response.IsSuccess)
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        
            return Ok(response);
        }
    }
}