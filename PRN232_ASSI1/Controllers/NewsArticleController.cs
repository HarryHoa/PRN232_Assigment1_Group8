using DLL.Interface;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_ASSI1.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NewsArticleController : ControllerBase
{
    private readonly INewArticleService _newArticleService;

    public NewsArticleController(INewArticleService newArticleService)
    {
        _newArticleService = newArticleService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var response = await _newArticleService.GetNewsInDateRangeAsync(startDate, endDate);
        if (!response.IsSuccess) return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        return Ok(response);
    }
}