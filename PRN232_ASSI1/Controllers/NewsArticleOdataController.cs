using DAL.Models;
using DLL.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace PRN232_ASSI1.Controllers;

[Route("odata/[controller]")]
[ApiController]
public class NewsArticleOdataController : ODataController
{
    
    private readonly INewsArticleService _newsArticleService;
    private readonly INewArticleService _newArticleService;
    private readonly ILogger<NewsArticleController> _logger;

    public NewsArticleOdataController(INewsArticleService newsArticleService, ILogger<NewsArticleController> logger,INewArticleService newArticleService)
    {
        _newsArticleService = newsArticleService;
        _logger = logger;
        _newArticleService = newArticleService;
    }
    
    // [EnableQuery]
    [HttpGet("statistics")]
    public async Task<IActionResult> GetAll(ODataQueryOptions<NewsArticle> odataOptions)
    {
        var response = await _newArticleService.GetNewsByODataAsync(odataOptions);
        if (!response.IsSuccess)
            return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        
        return Ok(response);
    }
}