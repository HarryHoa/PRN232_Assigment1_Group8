using DLL.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace PRN232_ASSI1.Controllers
{
    [Route("odata/[controller]")]
    public class CategoryODataController : ODataController
    {
        private readonly ICategoryService _service;

        public CategoryODataController(ICategoryService service)
        {
            _service = service;
        }

        [EnableQuery]
        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_service.GetQueryable());
        }
    }

}
