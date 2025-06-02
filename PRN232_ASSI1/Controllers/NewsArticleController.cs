using Common.Dto;
using Common.Dto.NewsArticleDto;
using DLL.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_ASS11.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Assuming authentication is required for staff
    public class NewsArticleController : ControllerBase
    {
        private readonly INewsArticleService _newsArticleService;
        private readonly INewArticleService _newArticleService;
        private readonly ILogger<NewsArticleController> _logger;

        public NewsArticleController(INewsArticleService newsArticleService, ILogger<NewsArticleController> logger)
        {
            _newsArticleService = newsArticleService;
            _logger = logger;
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

                // Get current user ID (you'll need to implement this based on your authentication system)
                var currentUserId = GetCurrentUserId();

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
                var currentUserId = GetCurrentUserId();

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
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi tải thẻ" });
            }
        }

        [HttpPost("bulk-delete")]
        public async Task<IActionResult> BulkDeleteNewsArticles([FromBody] List<string> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return BadRequest(new { message = "Invalid ID" });
                }

                int deletedCount = 0;
                var errors = new List<string>();

                foreach (var id in ids)
                {
                    try
                    {
                        var success = await _newsArticleService.DeleteNewsArticleAsync(id);
                        if (success)
                        {
                            deletedCount++;
                        }
                        else
                        {
                            errors.Add($"Cannot delete article with ID: {id}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error deleting news article with ID: {Id}", id);
                        errors.Add($"Lỗi khi xóa bài viết ID: {id}");
                    }
                }

                var response = new
                {
                    deletedCount,
                    totalRequested = ids.Count,
                    errors,
                    message = $"Delete {deletedCount}/{ids.Count} successfully"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during bulk delete operation");
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi xóa hàng loạt" });
            }
        }

        private short GetCurrentUserId()
        {

            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (short.TryParse(userIdClaim, out short userId))
            {
                return userId;
            }

            throw new UnauthorizedAccessException("Không thể xác định người dùng hiện tại");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            var response = await _newArticleService.GetNewsInDateRangeAsync(startDate, endDate);
            if (!response.IsSuccess) return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            return Ok(response);
        }
    }
}