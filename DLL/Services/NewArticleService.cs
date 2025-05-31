using AutoMapper;
using Common.Dto;
using DAL.Models;
using DAL.Repository;
using DLL.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DLL.Services;

public class NewArticleService : INewArticleService
{
    private readonly ILogger<NewArticleService> _logger;
    private readonly IGenericRepository<NewsArticle> _newsRepository;
    private readonly IMapper _mapper;

    public NewArticleService(IGenericRepository<NewsArticle> newsRepository, IMapper mapper,
        ILogger<NewArticleService> logger)
    {
        _newsRepository = newsRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ResponseDto> GetNewsInDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        try
        {
            _logger.LogInformation("start getting new articles from date range");
            var listNewArticle = await _newsRepository.GetAllAsync(query => query.Include(n => n.Category)
                .Where(n => n.CreatedDate.HasValue &&
                            n.CreatedDate.Value >= startDate &&
                            n.CreatedDate.Value <= endDate && 
                            n.NewsStatus == true)
                .OrderByDescending(n => n.CreatedDate));
            
            _logger.LogInformation("Start mapping data");
            var newArticleResList = _mapper.Map<List<ListNewArticleRes>>(listNewArticle);
            
            var totalPosts = newArticleResList.Count;
            var totalDays = (endDate - startDate).TotalDays + 1;
            var averagePerDay = (int)Math.Round(totalPosts / totalDays);

            var postsToday = newArticleResList.Count(x => x.CreatedDate.Date == DateTime.Today);
            var postsMonth = newArticleResList.Count(x => x.CreatedDate.Month == DateTime.Today.Month && x.CreatedDate.Year == DateTime.Today.Year);
            
            var result = new NewArticleRes
            {
                TotalPosts = totalPosts,
                AveragePerDay = averagePerDay,
                PostsMonth = postsMonth,
                PostsToday = postsToday,
                listArticle = newArticleResList
            };
            
            _logger.LogInformation("end getting new articles from date range");
            
            return new ResponseDto(
                statusCode: 200,
                message: "Retrieved new articles",
                isSuccess: true,
                result: result
            );
        }
        catch (Exception e)
        {
            _logger.LogError("Error at get new article cause by {}", e.Message);
            return new ResponseDto(
                statusCode: 500,
                message: "Error at get new article",
                isSuccess: false,
                result: null
            );
        }
    }
}