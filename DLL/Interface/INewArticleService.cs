using Common.Dto;

namespace DLL.Interface;

public interface INewArticleService
{
    Task<ResponseDto> GetNewsInDateRangeAsync(DateTime startDate, DateTime endDate);
}