using AutoMapper;
using Common.Dto;
using DAL.Models;
using DLL.Interface;
using Microsoft.EntityFrameworkCore;

namespace DLL.Services
{
    public class NewsArticleService : INewsArticleService
    {
        private readonly FUNewsManagementContext _context;
        private readonly IMapper _mapper;

        public NewsArticleService(FUNewsManagementContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<PagedResult<NewsArticleDto>> GetAllNewsArticlesAsync(NewsArticleSearchDto searchDto)
        {
            var query = _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .AsQueryable();

            // Filters
            if (!string.IsNullOrEmpty(searchDto.SearchTerm))
            {
                var searchTerm = searchDto.SearchTerm.ToLower();
                query = query.Where(n => 
                    n.NewsTitle.ToLower().Contains(searchTerm) ||
                    n.Headline.ToLower().Contains(searchTerm) ||
                    n.NewsContent.ToLower().Contains(searchTerm));
            }

            if (searchDto.CategoryId.HasValue)
            {
                query = query.Where(n => n.CategoryId == searchDto.CategoryId.Value);
            }

            if (searchDto.TagIds != null && searchDto.TagIds.Any())
            {
                query = query.Where(n => n.Tags.Any(t => searchDto.TagIds.Contains(t.TagId)));
            }

            if (searchDto.NewsStatus.HasValue)
            {
                query = query.Where(n => n.NewsStatus == searchDto.NewsStatus.Value);
            }

            if (searchDto.FromDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate >= searchDto.FromDate.Value);
            }

            if (searchDto.ToDate.HasValue)
            {
                query = query.Where(n => n.CreatedDate <= searchDto.ToDate.Value);
            }

            // Sorting
            query = searchDto.SortBy?.ToLower() switch
            {
                "title" => searchDto.SortOrder?.ToLower() == "asc" 
                    ? query.OrderBy(n => n.NewsTitle)
                    : query.OrderByDescending(n => n.NewsTitle),
                "category" => searchDto.SortOrder?.ToLower() == "asc"
                    ? query.OrderBy(n => n.Category.CategoryDesciption)
                    : query.OrderByDescending(n => n.Category.CategoryDesciption),
                "status" => searchDto.SortOrder?.ToLower() == "asc"
                    ? query.OrderBy(n => n.NewsStatus)
                    : query.OrderByDescending(n => n.NewsStatus),
                _ => searchDto.SortOrder?.ToLower() == "asc"
                    ? query.OrderBy(n => n.CreatedDate)
                    : query.OrderByDescending(n => n.CreatedDate)
            };

            var totalCount = await query.CountAsync();
            
            var newsArticles = await query
                .Skip((searchDto.Page - 1) * searchDto.PageSize)
                .Take(searchDto.PageSize)
                .ToListAsync();

            var items = _mapper.Map<List<NewsArticleDto>>(newsArticles);

            return new PagedResult<NewsArticleDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = searchDto.Page,
                PageSize = searchDto.PageSize
            };
        }

        public async Task<NewsArticleDto?> GetNewsArticleByIdAsync(string id)
        {
            var newsArticle = await _context.NewsArticles
                .Include(n => n.Category)
                .Include(n => n.CreatedBy)
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);

            if (newsArticle == null)
                return null;

            return _mapper.Map<NewsArticleDto>(newsArticle);
        }

        public async Task<string> CreateNewsArticleAsync(NewsArticleCreateDto createDto, short createdById)
        {
            var newsArticleId = Guid.NewGuid().ToString();
            
            var newsArticle = _mapper.Map<NewsArticle>(createDto);
            newsArticle.NewsArticleId = newsArticleId;
            newsArticle.CreatedById = createdById;
            newsArticle.CreatedDate = DateTime.Now;

            // Add tags if provided
            if (createDto.TagIds.Any())
            {
                var tags = await _context.Tags
                    .Where(t => createDto.TagIds.Contains(t.TagId))
                    .ToListAsync();
                
                newsArticle.Tags = tags;
            }

            _context.NewsArticles.Add(newsArticle);
            await _context.SaveChangesAsync();
            
            return newsArticleId;
        }

        public async Task<bool> UpdateNewsArticleAsync(NewsArticleUpdateDto updateDto, short updatedById)
        {
            var newsArticle = await _context.NewsArticles
                .Include(n => n.Tags)
                .FirstOrDefaultAsync(n => n.NewsArticleId == updateDto.NewsArticleId);

            if (newsArticle == null)
                return false;

            // Map update properties
            _mapper.Map(updateDto, newsArticle);
            newsArticle.UpdatedById = updatedById;
            newsArticle.ModifiedDate = DateTime.Now;

            // Update tags
            newsArticle.Tags.Clear();
            if (updateDto.TagIds.Any())
            {
                var tags = await _context.Tags
                    .Where(t => updateDto.TagIds.Contains(t.TagId))
                    .ToListAsync();
                
                newsArticle.Tags = tags;
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteNewsArticleAsync(string id)
        {
            var newsArticle = await _context.NewsArticles
                .FirstOrDefaultAsync(n => n.NewsArticleId == id);

            if (newsArticle == null)
                return false;

            _context.NewsArticles.Remove(newsArticle);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<CategoryDto>> GetCategoriesAsync()
        {
            var categories = await _context.Categories.ToListAsync();
            return _mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<List<TagDto>> GetTagsAsync()
        {
            var tags = await _context.Tags.ToListAsync();
            return _mapper.Map<List<TagDto>>(tags);
        }

        public async Task<bool> NewsArticleExistsAsync(string id)
        {
            return await _context.NewsArticles.AnyAsync(n => n.NewsArticleId == id);
        }
    }
}