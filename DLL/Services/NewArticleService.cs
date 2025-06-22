using AutoMapper;
using Common.Dto;
using DAL.Models;
using DAL.Repository;
using DLL.Interface;
using Microsoft.AspNetCore.OData.Query;
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
            var postsMonth = newArticleResList.Count(x =>
                x.CreatedDate.Month == DateTime.Today.Month && x.CreatedDate.Year == DateTime.Today.Year);

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

    public async Task<ResponseDto> GetNewsByODataAsync(ODataQueryOptions<NewsArticle> odataOptions)
    {
        _logger.LogInformation("Start getting new articles using odata");
        try
        {
            var listFromDb = await _newsRepository.GetAllAsync(q => q.Include(n => n.Category));
            _logger.LogInformation($"Records from DB: {listFromDb.Count()}");

            IQueryable<NewsArticle> queryableList = listFromDb.AsQueryable();
            IQueryable<NewsArticle> filteredQuery;
            try
            {
                if (odataOptions == null)
                {
                    _logger.LogWarning("OData options is null, returning all data");
                    filteredQuery = queryableList;
                }
                else
                {
                    filteredQuery = (IQueryable<NewsArticle>)odataOptions.ApplyTo(queryableList);

                    _logger.LogInformation($"OData filter applied successfully");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error applying OData filter, returning all data");
                filteredQuery = queryableList;
            }

            var filteredList = filteredQuery.ToList();
            _logger.LogInformation($"Records after OData filter: {filteredList.Count}");

            if (filteredList.Count == 0 && listFromDb.Count() > 0)
            {
                filteredList = listFromDb.ToList();
            }

            var newArticleResList = _mapper.Map<List<ListNewArticleRes>>(filteredList);

            var createdDates = newArticleResList
                .Where(x => x.CreatedDate != default && x.CreatedDate != DateTime.MinValue)
                .Select(x => x.CreatedDate.Date)
                .ToList();

            DateTime? startDate = createdDates.Any() ? createdDates.Min() : null;
            DateTime? endDate = createdDates.Any() ? createdDates.Max() : null;

            _logger.LogInformation(
                $"Start Date: {startDate?.ToString("yyyy-MM-dd") ?? "null"}, End Date: {endDate?.ToString("yyyy-MM-dd") ?? "null"}");

            var totalPosts = newArticleResList.Count;
            var totalDays = (startDate.HasValue && endDate.HasValue)
                ? Math.Max(1, (endDate.Value - startDate.Value).TotalDays + 1) // Ensure at least 1 day
                : 1;

            var averagePerDay = totalDays > 0 ? (int)Math.Round((double)totalPosts / totalDays) : 0;

            var postsToday = newArticleResList.Count(x =>
                x.CreatedDate.Date == DateTime.Today);

            var postsMonth = newArticleResList.Count(x =>
                x.CreatedDate.Month == DateTime.Today.Month &&
                x.CreatedDate.Year == DateTime.Today.Year);

            var result = new NewArticleRes
            {
                TotalPosts = totalPosts,
                AveragePerDay = averagePerDay,
                PostsMonth = postsMonth,
                PostsToday = postsToday,
                listArticle = newArticleResList
            };

            _logger.LogInformation(
                $"End getting new articles - Total: {totalPosts}, Today: {postsToday}, Month: {postsMonth}");

            return new ResponseDto(
                statusCode: 200,
                message: "Retrieved new articles",
                isSuccess: true,
                result: result
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error in GetNewArticles method");
            return new ResponseDto(
                statusCode: 500,
                message: "An error occurred while retrieving articles",
                isSuccess: false,
                result: null
            );
        }
    }
}