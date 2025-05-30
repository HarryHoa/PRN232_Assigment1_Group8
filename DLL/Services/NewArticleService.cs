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
                            n.CreatedDate.Value <= endDate)
                .OrderByDescending(n => n.CreatedDate));

            var listNewArticleRes = _mapper.Map<IList<NewArticleRes>>(listNewArticle);
            return new ResponseDto(
                statusCode: 200,
                message: "Retrieved new articles",
                isSuccess: true,
                result: listNewArticleRes
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