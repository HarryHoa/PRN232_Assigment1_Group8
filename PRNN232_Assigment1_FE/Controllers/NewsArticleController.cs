using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Common.Dto;
using Common.Dto.NewsArticleDto;
using DAL.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PRNN232_Assigment1_FE.Models;

namespace PRNN232_Assigment1_FE.Controllers;
[Authorize(Roles = "1")]

public class NewsArticlesController : Controller
{
    private readonly HttpClient _httpClient;
    private readonly FUNewsManagementContext _context;

    public NewsArticlesController(HttpClient httpClient, FUNewsManagementContext context)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("https://localhost:7252/api/");
        _context = context;
    }

    // GET: NewsArticles
// GET: NewsArticles
public async Task<IActionResult> Index(string searchTerm, short? categoryId, bool? newsStatus, 
                                     DateTime? fromDate, int page = 1, int pageSize = 10)
{
    try
    {
        // Build OData query
        var baseUrl = "/odata/NewsArticleOData";
        var queryParams = new List<string>();

        // Add $expand to include related data
        queryParams.Add("$expand=Category,CreatedBy,Tags");
        
        // Add filters
        var filters = new List<string>();
        
        if (!string.IsNullOrEmpty(searchTerm))
        {
            filters.Add($"contains(tolower(NewsTitle),'{searchTerm.ToLower()}') or contains(tolower(NewsContent),'{searchTerm.ToLower()}')");
        }
        
        if (categoryId.HasValue)
        {
            filters.Add($"CategoryId eq {categoryId.Value}");
        }
        
        if (newsStatus.HasValue)
        {
            filters.Add($"NewsStatus eq {newsStatus.Value.ToString().ToLower()}");
        }
        
        if (fromDate.HasValue)
        {
            var dateString = fromDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ");
            filters.Add($"CreatedDate ge {dateString}");
        }

        if (filters.Any())
        {
            queryParams.Add($"$filter={Uri.EscapeDataString(string.Join(" and ", filters))}");
        }

        // Add ordering
        queryParams.Add("$orderby=CreatedDate desc");
        
        // Add pagination
        queryParams.Add($"$top={pageSize}");
        queryParams.Add($"$skip={(page - 1) * pageSize}");
        
        // Add count
        queryParams.Add("$count=true");

        string queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";
        string apiUrl = baseUrl + queryString;

        Console.WriteLine($"Calling OData API URL: {apiUrl}");

        // Call OData API
        var response = await _httpClient.GetAsync(apiUrl);

        if (response.IsSuccessStatusCode)
        {
            var jsonData = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"OData Response: {jsonData}"); // DEBUG: Log the response

            List<NewsArticleODataItem> newsArticles = new List<NewsArticleODataItem>();
            int totalCount = 0;

            try
            {
                // ✅ FIX: Handle both array and object responses
                using (JsonDocument document = JsonDocument.Parse(jsonData))
                {
                    var root = document.RootElement;
                    
                    if (root.ValueKind == JsonValueKind.Array)
                    {
                        // Case 1: Direct array response (no OData metadata)
                        Console.WriteLine("Detected direct array response");
                        
                        newsArticles = JsonSerializer.Deserialize<List<NewsArticleODataItem>>(jsonData,
                            new JsonSerializerOptions { 
                                PropertyNameCaseInsensitive = true
                            }) ?? new List<NewsArticleODataItem>();
                        
                        totalCount = newsArticles.Count;
                    }
                    else if (root.ValueKind == JsonValueKind.Object)
                    {
                        // Case 2: OData response with metadata
                        Console.WriteLine("Detected OData metadata response");
                        
                        if (root.TryGetProperty("value", out JsonElement valueElement))
                        {
                            var newsArticleJson = valueElement.GetRawText();
                            newsArticles = JsonSerializer.Deserialize<List<NewsArticleODataItem>>(newsArticleJson,
                                new JsonSerializerOptions { 
                                    PropertyNameCaseInsensitive = true
                                }) ?? new List<NewsArticleODataItem>();
                        }
                        
                        if (root.TryGetProperty("@odata.count", out JsonElement countElement))
                        {
                            totalCount = countElement.GetInt32();
                        }
                        else
                        {
                            totalCount = newsArticles.Count;
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Unexpected JSON structure. Root type: {root.ValueKind}");
                        throw new InvalidOperationException($"Unexpected JSON structure: {root.ValueKind}");
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                Console.WriteLine($"JSON parsing error: {jsonEx.Message}");
                
                // Fallback: Try to parse as simple array of NewsArticle (your entity model)
                try
                {
                    var fallbackArticles = JsonSerializer.Deserialize<List<DAL.Models.NewsArticle>>(jsonData,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    if (fallbackArticles != null)
                    {
                        // Convert from entity model to OData item model
                        newsArticles = fallbackArticles.Select(entity => new NewsArticleODataItem
                        {
                            NewsArticleId = entity.NewsArticleId,
                            NewsTitle = entity.NewsTitle,
                            Headline = entity.Headline,
                            NewsContent = entity.NewsContent,
                            NewsSource = entity.NewsSource,
                            CategoryId = entity.CategoryId,
                            NewsStatus = entity.NewsStatus,
                            CreatedDate = entity.CreatedDate,
                            ModifiedDate = entity.ModifiedDate,
                            CreatedById = entity.CreatedById,
                            UpdatedById = entity.UpdatedById,
                            Category = entity.Category != null ? new CategoryODataItem
                            {
                                CategoryId = entity.Category.CategoryId,
                                CategoryName = entity.Category.CategoryName,
                                CategoryDesciption = entity.Category.CategoryDesciption
                            } : null,
                            CreatedBy = entity.CreatedBy != null ? new SystemAccountODataItem
                            {
                                AccountId = entity.CreatedBy.AccountId,
                                AccountName = entity.CreatedBy.AccountName,
                                AccountEmail = entity.CreatedBy.AccountEmail
                            } : null,
                            Tags = entity.Tags?.Select(tag => new TagODataItem
                            {
                                TagId = tag.TagId,
                                TagName = tag.TagName,
                                Note = tag.Note
                            }).ToList() ?? new List<TagODataItem>()
                        }).ToList();
                        
                        totalCount = newsArticles.Count;
                        Console.WriteLine($"Fallback parsing successful. Found {newsArticles.Count} articles");
                    }
                }
                catch (Exception fallbackEx)
                {
                    Console.WriteLine($"Fallback parsing also failed: {fallbackEx.Message}");
                    throw new InvalidOperationException($"Unable to parse JSON response: {jsonEx.Message}", jsonEx);
                }
            }

            // Convert OData items to DTOs
            var newsArticleDtos = newsArticles.Select(item => new NewsArticleDto
            {
                NewsArticleId = item.NewsArticleId ?? "",
                Title = item.NewsTitle ?? "",
                Headline = item.Headline ?? "",
                Content = item.NewsContent ?? "",
                Source = item.NewsSource ?? "",
                CategoryId = item.CategoryId ?? 0,
                CategoryName = item.Category?.CategoryDesciption ?? item.Category?.CategoryName ?? "",
                Status = item.NewsStatus ?? false,
                CreatedDate = item.CreatedDate,
                ModifiedDate = item.ModifiedDate,
                CreatedByName = item.CreatedBy?.AccountName ?? "",
                TagIds = item.Tags?.Select(t => t.TagId).ToList() ?? new List<int>(),
                Tags = item.Tags?.Select(t => new TagDto 
                { 
                    TagId = t.TagId, 
                    TagName = t.TagName ?? "", 
                    Note = t.Note 
                }).ToList() ?? new List<TagDto>()
            }).ToList();

            // Convert to PagedResult for existing view
            var pagedResult = new PagedResult<NewsArticleDto>
            {
                Items = newsArticleDtos,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };

            Console.WriteLine($"Successfully parsed {newsArticleDtos.Count} articles with total count: {totalCount}");

            // Load categories for dropdown
            await LoadCategoriesForViewBag();

            return View(pagedResult);
        }
        else
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"OData API Error: {response.StatusCode} - {errorContent}");
            
            TempData["Error"] = "Không thể lấy dữ liệu bài viết từ OData API.";
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
        TempData["Error"] = $"Lỗi khi gọi OData API: {ex.Message}";
        Console.WriteLine($"Error calling OData API: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        
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
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            // return View();
        }
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