using System.Text.Json;
using Common.Dto;
using Microsoft.AspNetCore.Mvc;

namespace PRNN232_Assigment1_FE.Controllers
{
    public class NewsController : Controller
    {

        private readonly HttpClient _httpClient;

        public NewsController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7252/api/");
        }
        
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, string category)
        {
            var from = startDate ?? DateTime.Today.AddDays(-6);
            var to = endDate ?? DateTime.Today;

            var requestUrl = $"NewsArticle?startDate={from:yyyy-MM-dd}&endDate={to:yyyy-MM-dd}";
            Console.WriteLine("Calling API URL: " + _httpClient.BaseAddress + requestUrl);

            var response = await _httpClient.GetAsync(requestUrl);

            var responseJson = await response.Content.ReadAsStringAsync();

            var responseDto = JsonSerializer.Deserialize<ResponseDto>(responseJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            ViewBag.StartDate = from.ToString("yyyy-MM-dd");
            ViewBag.EndDate = to.ToString("yyyy-MM-dd");
            if (responseDto != null && responseDto.IsSuccess)
            {
                var resultJson = responseDto.Result?.ToString();
                if (!string.IsNullOrEmpty(resultJson))
                {
                    var newsList = JsonSerializer.Deserialize<NewArticleRes>(resultJson);
                    ViewBag.TotalPosts = newsList?.TotalPosts;
                    ViewBag.AveragePerDay = newsList?.AveragePerDay;
                    ViewBag.PostsMonth = newsList?.PostsMonth;
                    ViewBag.PostsToday = newsList?.PostsToday;
                    
                    return View(newsList?.listArticle ?? new List<ListNewArticleRes>());

                }
            }

            return View();

        }
    }
} 