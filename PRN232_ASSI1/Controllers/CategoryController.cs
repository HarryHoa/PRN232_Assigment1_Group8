using Common.Dto.CategoryDto;
using Common.Dto.CategoryDTO;
using DAL.Models;
using DLL.Interface;
using DLL.Services;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_ASSI1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _service;

        public CategoryController(ICategoryService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _service.GetAllAsync();
            return Ok(categories);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(short id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category == null) return NotFound();
            return Ok(category);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryCreateDto dto)
        {
            var created = await _service.AddAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.CategoryId }, created);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(short id, CategoryUpdateDto dto)
        {
          

            try
            {
                await _service.UpdateAsync(id,dto);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(short id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string keyword)
        {
            var results = await _service.SearchAsync(keyword);
            return Ok(results);
        }
        [HttpGet("{categoryId}/articles")]
        public async Task<IActionResult> GetArticlesByCategoryId(short categoryId)
        {
            var articles = await _service.GetArticlesByCategoryIdAsync(categoryId);
            if (articles == null || !articles.Any())
                return NotFound("No articles found for this category");
            return Ok(articles);
        }
        [HttpGet("categories/{parentId}/subcategories")]
        public async Task<IActionResult> GetSubCategories(short parentId)
        {
            var result = await _service.GetSubCategoriesAsync(parentId);
            return Ok(result);
        }


    }


}
