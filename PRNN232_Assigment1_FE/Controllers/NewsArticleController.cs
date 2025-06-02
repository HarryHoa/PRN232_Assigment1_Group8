using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Common.Dto;
using Common.Dto.NewsArticleDto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PRNN232_Assigment1_FE.Controllers;

public class NewsArticlesController : Controller
{
    private readonly HttpClient _httpClient;

    public NewsArticlesController(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7252/api/");
    }

    // GET: NewsArticles
    public async Task<IActionResult> Index(string searchTerm, short? categoryId, bool? newsStatus, 
                                         DateTime? fromDate, int page = 1, int pageSize = 10)
    {
        try
        {
            // Tạo NewsArticleSearchDto object để gửi lên API
            var searchDto = new NewsArticleSearchDto
            {
                SearchTerm = searchTerm,
                CategoryId = categoryId,
                NewsStatus = newsStatus,
                FromDate = fromDate,
                Page = page,
                PageSize = pageSize
            };

            // Tạo query string cho API
            var queryParams = new List<string>();
            
            if (!string.IsNullOrEmpty(searchDto.SearchTerm))
                queryParams.Add($"searchTerm={Uri.EscapeDataString(searchDto.SearchTerm)}");
            
            if (searchDto.CategoryId.HasValue)
                queryParams.Add($"categoryId={searchDto.CategoryId.Value}");
            
            if (searchDto.NewsStatus.HasValue)
                queryParams.Add($"newsStatus={searchDto.NewsStatus.Value}");
            
            if (searchDto.FromDate.HasValue)
                queryParams.Add($"fromDate={searchDto.FromDate.Value:yyyy-MM-dd}");
            
            queryParams.Add($"page={searchDto.Page}");
            queryParams.Add($"pageSize={searchDto.PageSize}");

            string queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
            string apiUrl = $"/api/NewsArticle{queryString}";

            Console.WriteLine($"Calling API URL: {apiUrl}");

            // Gọi API để lấy danh sách bài viết
            var response = await _httpClient.GetAsync(apiUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var pagedResult = JsonSerializer.Deserialize<PagedResult<NewsArticleDto>>(jsonData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                // Lấy danh sách categories cho dropdown
                await LoadCategoriesForViewBag();

                return View(pagedResult);
            }
            else
            {
                TempData["Error"] = "Không thể lấy dữ liệu bài viết từ API.";
                await LoadCategoriesForViewBag();
                return View(new PagedResult<NewsArticleDto>
                {
                    Items = new List<NewsArticleDto>(),
                    Page = 1,
                    PageSize = pageSize,
                    TotalCount = 0
                });
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi gọi API: {ex.Message}";
            Console.WriteLine($"Error calling NewsArticle API: {ex.Message}");
            await LoadCategoriesForViewBag();
            return View(new PagedResult<NewsArticleDto>
            {
                Items = new List<NewsArticleDto>(),
                Page = 1,
                PageSize = pageSize,
                TotalCount = 0
            });
        }
    }

    // GET: NewsArticles/Details/5
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        try
        {
            var response = await _httpClient.GetAsync($"/api/NewsArticle/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var newsArticle = JsonSerializer.Deserialize<NewsArticleDto>(jsonData, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (newsArticle == null)
                    return NotFound();

                return View(newsArticle);
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi lấy chi tiết bài viết: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // GET: NewsArticles/Create
    public async Task<IActionResult> Create()
    {
        await LoadCategoriesAndTagsForCreate();
        return View(new NewsArticleCreateDto());
    }

    // POST: NewsArticles/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(NewsArticleCreateDto createDto)
    {
        if (!ModelState.IsValid)
        {
            await LoadCategoriesAndTagsForCreate();
            return View(createDto);
        }
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        Console.WriteLine($"User.Identity.IsAuthenticated: {User.Identity.IsAuthenticated}");
        Console.WriteLine($"UserIdClaim: {userIdClaim}");
        if (short.TryParse(userIdClaim, out short currentUserId))
        {
            createDto.CurrentUserId = currentUserId; // Gán vào DTO
        }
        try
        {
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(createDto),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync("/api/NewsArticle", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Tạo bài viết thành công!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"Tạo bài viết thất bại: {response.StatusCode}");
                await LoadCategoriesAndTagsForCreate();
                return View(createDto);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Lỗi gọi API: {ex.Message}");
            await LoadCategoriesAndTagsForCreate();
            return View(createDto);
        }
    }

    // GET: NewsArticles/Edit/5
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            return NotFound();
        }

        try
        {
            var response = await _httpClient.GetAsync($"/api/NewsArticle/{id}");

            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var newsArticle = JsonSerializer.Deserialize<NewsArticleDto>(jsonData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                if (newsArticle == null)
                    return NotFound();

                // Convert to UpdateDto
                var updateDto = new NewsArticleUpdateDto
                {
                    NewsArticleId = newsArticle.NewsArticleId,
                    Title = newsArticle.Title,
                    Headline = newsArticle.Headline,
                    Content = newsArticle.Content,
                    Source = newsArticle.Source,
                    CategoryId = newsArticle.CategoryId,
                    Status = newsArticle.Status,
                    Tags = newsArticle.Tags
                };

                await LoadCategoriesAndTagsForCreate();
                return View(updateDto);
            }
            else
            {
                return NotFound();
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi lấy thông tin bài viết: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: NewsArticles/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, NewsArticleUpdateDto updateDto)
    {
        if (id != updateDto.NewsArticleId)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            await LoadCategoriesAndTagsForCreate();
            return View(updateDto);
        }
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (short.TryParse(userIdClaim, out short currentUserId))
        {
            updateDto.CurrentUserId = currentUserId; // Gán vào DTO
        }
        try
        {
            var jsonContent = new StringContent(
                JsonSerializer.Serialize(updateDto),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PutAsync($"/api/NewsArticle/{id}", jsonContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Cập nhật bài viết thành công!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Cập nhật bài viết thất bại.");
                await LoadCategoriesAndTagsForCreate();
                return View(updateDto);
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Lỗi gọi API: {ex.Message}");
            await LoadCategoriesAndTagsForCreate();
            return View(updateDto);
        }
    }

    // GET: NewsArticles/Delete/5
    public async Task<IActionResult> Delete(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        try
        {
            var response = await _httpClient.GetAsync($"/api/NewsArticle/{id}");
            
            if (!response.IsSuccessStatusCode)
                return NotFound();

            var jsonData = await response.Content.ReadAsStringAsync();
            var newsArticle = JsonSerializer.Deserialize<NewsArticleDto>(jsonData,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (newsArticle == null)
                return NotFound();

            return View(newsArticle);
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi lấy thông tin bài viết: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    // POST: NewsArticles/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"/api/NewsArticle/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Xóa bài viết thành công!";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["Error"] = "Không thể xóa bài viết.";
                return RedirectToAction(nameof(Index));
            }
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Lỗi khi xóa bài viết: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }

    private async Task LoadCategoriesForViewBag()
    {
        try
        {
            var response = await _httpClient.GetAsync("/api/NewsArticle/categories");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<CategoryDto>>(jsonData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ViewBag.Categories = categories ?? new List<CategoryDto>();
            }
            else
            {
                ViewBag.Categories = new List<CategoryDto>();
            }
        }
        catch
        {
            ViewBag.Categories = new List<CategoryDto>();
        }
    }

    private async Task LoadCategoriesAndTagsForCreate()
    {
        try
        {
            // Load Categories
            var response = await _httpClient.GetAsync("/api/NewsArticle/categories");
            if (response.IsSuccessStatusCode)
            {
                var jsonData = await response.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<CategoryDto>>(jsonData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            
                // Convert to SelectList
                ViewBag.Categories = new SelectList(
                    categories ?? new List<CategoryDto>(), 
                    "CategoryId", 
                    "CategoryName"
                );
            }
            else
            {
                ViewBag.Categories = new SelectList(new List<CategoryDto>(), "CategoryId", "CategoryName");
            }
        
            // Load Tags
            var tagsResponse = await _httpClient.GetAsync("/api/NewsArticle/tags");
            if (tagsResponse.IsSuccessStatusCode)
            {
                var tagsJsonData = await tagsResponse.Content.ReadAsStringAsync();
                var tags = JsonSerializer.Deserialize<List<TagDto>>(tagsJsonData,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ViewBag.Tags = tags?.Select(t => new SelectListItem
                {
                    Value = t.TagId.ToString(),
                    Text = t.TagName
                }).ToList() ?? new List<SelectListItem>();
            }
            else
            {
                ViewBag.Tags = new List<SelectListItem>();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading categories and tags: {ex.Message}");
            ViewBag.Categories = new SelectList(new List<CategoryDto>(), "CategoryId", "CategoryName");
            ViewBag.Tags = new List<SelectListItem>();
        }
    }
}