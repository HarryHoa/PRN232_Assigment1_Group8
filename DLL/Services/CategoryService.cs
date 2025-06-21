using Common.Dto.CategoryDto;
using Common.Dto.CategoryDTO;
using DAL.Models;
using DLL.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly FUNewsManagementContext _context;

        public CategoryService(FUNewsManagementContext context)
        {
            _context = context;
        }

        public async Task<List<CategoryResponseDto>> GetAllAsync()
        {

            return await _context.Categories
                        .Select(c => new CategoryResponseDto
                        {
                            CategoryId = c.CategoryId,
                            CategoryName = c.CategoryName,
                            CategoryDesciption = c.CategoryDesciption,
                            IsActive = c.IsActive,
                            ParentCategoryId = c.ParentCategoryId,
                            ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.CategoryName : null
                        })
                        .ToListAsync();
        }

        public async Task<CategoryResponseDto> GetByIdAsync(short id)
        {
            var category = await _context.Categories
                                 .Include(c => c.ParentCategory) // Nếu cần lấy tên cha
                                 .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null) return null;

            return new CategoryResponseDto
            {
                CategoryId = category.CategoryId,
                CategoryName = category.CategoryName,
                CategoryDesciption = category.CategoryDesciption,
                IsActive = category.IsActive,
                ParentCategoryId = category.ParentCategoryId,
                ParentCategoryName = category.ParentCategory != null ? category.ParentCategory.CategoryName : null
            };
        }

        public async Task<Category> AddAsync(CategoryCreateDto category)
        {
            // Mặc định IsActive = true khi thêm mới
            
            Category newCategory= new Category
            {
                CategoryName = category.CategoryName,
                CategoryDesciption = category.CategoryDesciption,
                ParentCategoryId = category.ParentCategoryId,
            };
            newCategory.IsActive = true;

            _context.Categories.Add(newCategory);
            await _context.SaveChangesAsync();
            return newCategory;
        }

        public async Task UpdateAsync(short id, CategoryUpdateDto category)
        {
            var existing = await _context.Categories.FindAsync(id);
            if (existing == null)
            {
                throw new KeyNotFoundException("Category not found");
            }

            // Cập nhật các trường
            existing.CategoryName = category.CategoryName;
            existing.CategoryDesciption = category.CategoryDesciption;
            existing.ParentCategoryId = category.ParentCategoryId;
            existing.IsActive = category.IsActive ?? existing.IsActive;
            //await _context.Update(existing);                                                       
            await _context.SaveChangesAsync();
            
        }

        public async Task DeleteAsync(short id)
        {
            var category = await _context.Categories
                                        .Include(c => c.NewsArticles)
                                        .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category == null)
                throw new Exception("Category not found");

            if (category.NewsArticles != null && category.NewsArticles.Any())
                throw new Exception("Cannot delete category with existing news articles");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }

        public async Task<List<CategoryResponseDto>> SearchAsync(string keyword)
        {
            return await _context.Categories
                .Where(c => c.CategoryName.Contains(keyword))
                 .Select(c => new CategoryResponseDto
                 {
                     CategoryId = c.CategoryId,
                     CategoryName = c.CategoryName,
                     CategoryDesciption = c.CategoryDesciption,
                     IsActive = c.IsActive,
                     ParentCategoryId = c.ParentCategoryId,
                     ParentCategoryName = c.ParentCategory != null ? c.ParentCategory.CategoryName : null
                 })
                        .ToListAsync();
        }
        // hàm lấy danh sách artical của 1 category
        public async Task<List<NewsArticle>> GetArticlesByCategoryIdAsync(short categoryId)
        {
            return await _context.NewsArticles
                .Where(a => a.CategoryId == categoryId)
                .ToListAsync();
        }
        // hàm lấy những danh mục con của 1 parent category
        public async Task<List<CategoryResponseDto>> GetSubCategoriesAsync(short parentId)
        {
            return await _context.Categories
                .Where(c => c.ParentCategoryId == parentId)
                .Select(c => new CategoryResponseDto
                {
                    CategoryId = c.CategoryId,
                    CategoryName = c.CategoryName,
                    CategoryDesciption = c.CategoryDesciption,
                    IsActive = c.IsActive,
                    ParentCategoryId = c.ParentCategoryId
                })
                .ToListAsync();
        }

        public IQueryable<Category> GetQueryable()
        {
            return _context.Categories.AsQueryable();
        }
    }
}
