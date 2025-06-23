using System.Security.Claims;
using Common.Dto;
using Common.Dto.NewsArticleDto;
using DAL.Models;
using DLL.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;

namespace PRN232_ASSI1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    // [Authorize(Roles =("Staff"))] // Assuming authentication is required for staff
    public class NewsArticleController : ControllerBase
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly INewArticleService _newArticleService;
        private readonly ILogger<NewsArticleController> _logger;

        public NewsArticleController(INewsArticleService newsArticleService, ILogger<NewsArticleController> logger,INewArticleService newArticleService)
        {
            _newsArticleService = newsArticleService;
            _logger = logger;
            _newArticleService = newArticleService;
        }

        /// <summary>
        /// Get all news articles with search and pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PagedResult<NewsArticleDto>>> GetNewsArticles(
            [FromQuery] NewsArticleSearchDto searchDto)
        {
            try
            {
                var result = await _newsArticleService.GetAllNewsArticlesAsync(searchDto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting news articles");
                return StatusCode(500, new { message = "Error occurred while getting news articles" });
            }
        }

        /// <summary>
        /// Get news article by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<NewsArticleDto>> GetNewsArticle(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "Invalid Id" });
                }

                var newsArticle = await _newsArticleService.GetNewsArticleByIdAsync(id);

                if (newsArticle == null)
                {
                    return NotFound(new { message = "Can't find News Article" });
                }

                return Ok(newsArticle);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting news article with ID: {Id}", id);
                return StatusCode(500, new { message = "There are problems when getting new article" });
            }
        }

        /// <summary>
        /// Create new news article
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<string>> CreateNewsArticle([FromBody] NewsArticleCreateDto createDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                
                var currentUserId = createDto.CurrentUserId;

                var newsArticleId = await _newsArticleService.CreateNewsArticleAsync(createDto, currentUserId);

                return CreatedAtAction(
                    nameof(GetNewsArticle),
                    new { id = newsArticleId },
                    new { id = newsArticleId, message = "Success" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating news article");
                return StatusCode(500, new { message = "Error occurred while creating news article" });
            }
        }

        /// <summary>
        /// Update existing news article
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNewsArticle(string id, [FromBody] NewsArticleUpdateDto updateDto)
        {
            try
            {
                if (id != updateDto.NewsArticleId)
                {
                    return BadRequest(new { message = "ID bài viết không khớp" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var exists = await _newsArticleService.NewsArticleExistsAsync(id);
                if (!exists)
                {
                    return NotFound(new { message = "Không tìm thấy bài viết cần cập nhật" });
                }

                // Get current user ID
                var currentUserId = GetCurrentUserId(updateDto);

                var success = await _newsArticleService.UpdateNewsArticleAsync(updateDto, currentUserId);

                if (!success)
                {
                    return StatusCode(500, new { message = "Không thể cập nhật bài viết" });
                }

                return Ok(new { message = "Cập nhật bài viết thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating news article with ID: {Id}", id);
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật bài viết" });
            }
        }

        /// <summary>
        /// Delete news article
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNewsArticle(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return BadRequest(new { message = "ID bài viết không hợp lệ" });
                }

                var exists = await _newsArticleService.NewsArticleExistsAsync(id);
                if (!exists)
                {
                    return NotFound(new { message = "Không tìm thấy bài viết cần xóa" });
                }

                var success = await _newsArticleService.DeleteNewsArticleAsync(id);

                if (!success)
                {
                    return StatusCode(500, new { message = "Không thể xóa bài viết" });
                }

                return Ok(new { message = "Xóa bài viết thành công" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting news article with ID: {Id}", id);
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa bài viết" });
            }
        }

        /// <summary>
        /// Get all categories
        /// </summary>
        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            try
            {
                var categories = await _newsArticleService.GetCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting categories");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tải danh mục" });
            }
        }

        /// <summary>
        /// Get all tags
        /// </summary>
        [HttpGet("tags")]
        public async Task<ActionResult<List<TagDto>>> GetTags()
        {
            try
            {
                var tags = await _newsArticleService.GetTagsAsync();
                return Ok(tags);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting tags");
                return StatusCode(500, new { message = "Error when get tags" });
            }
        }

        private short GetCurrentUserId(NewsArticleUpdateDto updateDto)
        {
            return updateDto.CurrentUserId;
        }
        
        [HttpGet("statistics")]
        public async Task<IActionResult> GetAll([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var response = await _newArticleService.GetNewsInDateRangeAsync(startDate, endDate);
            if (!response.IsSuccess) return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            return Ok(response);
        }
        [HttpGet("odata-query")]
        public async Task<IActionResult> GetWithODataQuery(
            [FromQuery] string? filter = null,
            [FromQuery] string? orderby = null,
            [FromQuery] int? top = null,
            [FromQuery] int? skip = null,
            [FromQuery] bool count = false)
        {
            try
            {
                var baseQuery = $"odata/NewsArticleOData";
                var queryParams = new List<string>();

                if (!string.IsNullOrEmpty(filter))
                    queryParams.Add($"$filter={Uri.EscapeDataString(filter)}");
        
                if (!string.IsNullOrEmpty(orderby))
                    queryParams.Add($"$orderby={Uri.EscapeDataString(orderby)}");
        
                if (top.HasValue)
                    queryParams.Add($"$top={top.Value}");
        
                if (skip.HasValue)
                    queryParams.Add($"$skip={skip.Value}");
        
                if (count)
                    queryParams.Add("$count=true");

                var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
                var fullQuery = baseQuery + queryString;

                return Ok(new { 
                    message = "Use this OData query", 
                    odataUrl = fullQuery,
                    example = "odata/NewsArticleOData?$filter=contains(NewsTitle,'news')&$orderby=CreatedDate desc&$top=10&$count=true"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error building OData query", error = ex.Message });
            }
        }


    }
}