using Common.Dto.CategoryDto;
using Common.Dto.CategoryDTO;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLL.Interface
{
    public interface ICategoryService
    {
        IQueryable<Category> GetQueryable();
        Task<List<CategoryResponseDto>> GetAllAsync();
        Task<CategoryResponseDto> GetByIdAsync(short id);
        Task<Category> AddAsync(CategoryCreateDto category);
        Task UpdateAsync(short id,CategoryUpdateDto category);
        Task DeleteAsync(short id);
        Task<List<CategoryResponseDto>> SearchAsync(string keyword);
        Task<List<NewsArticle>> GetArticlesByCategoryIdAsync(short categoryId);
        Task<List<CategoryResponseDto>> GetSubCategoriesAsync(short parentId);
    }
}
