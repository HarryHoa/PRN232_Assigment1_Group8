using Common.Dto;
using DAL.Models;
using Microsoft.AspNetCore.OData.Query;

namespace DLL.Interface;

public interface INewArticleService
{
    Task<ResponseDto> GetNewsInDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<ResponseDto> GetNewsByODataAsync(ODataQueryOptions<NewsArticle> odataOptions);
}