using Common.Dto;
using Common.Dto.NewsArticleDto;

namespace DLL.Interface
{
    public interface INewsArticleService
    {
        Task<PagedResult<NewsArticleDto>> GetAllNewsArticlesAsync(NewsArticleSearchDto searchDto);
        Task<NewsArticleDto?> GetNewsArticleByIdAsync(string id);
        Task<string> CreateNewsArticleAsync(NewsArticleCreateDto createDto, short createdById);
        Task<bool> UpdateNewsArticleAsync(NewsArticleUpdateDto updateDto, short updatedById);
        Task<bool> DeleteNewsArticleAsync(string id);
        Task<List<CategoryDto>> GetCategoriesAsync();
        Task<List<TagDto>> GetTagsAsync();
        Task<bool> NewsArticleExistsAsync(string id);
    }
}
