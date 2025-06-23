using DLL.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace PRN232_ASSI1.Controllers
{
    [Route("odata/[controller]")]
    public class AdminCrudAccountODataController : ODataController
    {
        private readonly IAdminCrudAccountService _service;

        public AdminCrudAccountODataController(IAdminCrudAccountService service)
        {
            _service = service;
        }

        [HttpGet]
        [EnableQuery]
        public IActionResult Get()
        {
            var data = _service.GetAllForOdata(); // trả về IQueryable<AdminCRUDdto>
            return Ok(data);
        }
    }

}
