using Common.Dto;
using DLL.Interface;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace PRN232_ASSI1.Controllers
{
    [ApiController]
    [Route("odata/[controller]")]
    public class AdminCrudAccountController : ODataController
    {
        private readonly IAdminCrudAccountService _service;

        public AdminCrudAccountController(IAdminCrudAccountService service)
        {
            _service = service;
        }

        // GET: odata/AdminCrudAccount
        [EnableQuery]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var response = await _service.GetAllAsync();

            if (!response.IsSuccess)
                return StatusCode((int)response.StatusCode, response.Message);

            return Ok(response.Result); // thường là danh sách IQueryable<AdminCRUDdto>
        }

        // GET: odata/AdminCrudAccount(1)
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(short id)
        {
            var response = await _service.GetByIdAsync(id);

            if (!response.IsSuccess)
                return StatusCode((int)response.StatusCode, response.Message);

            return Ok(response.Result);
        }

        // POST: odata/AdminCrudAccount
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminCRUDdto dto)
        {
            var response = await _service.CreateAsync(dto);

            if (!response.IsSuccess)
                return StatusCode((int)response.StatusCode, response.Message);

            return Ok(response.Result);
        }

        // PUT: odata/AdminCrudAccount(1)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(short id, [FromBody] AdminCRUDdto dto)
        {
            var response = await _service.UpdateAsync(id, dto);

            if (!response.IsSuccess)
                return StatusCode((int)response.StatusCode, response.Message);

            return Ok(response.Result);
        }

        // DELETE: odata/AdminCrudAccount(1)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(short id)
        {
            var response = await _service.DeleteAsync(id);

            if (!response.IsSuccess)
                return StatusCode((int)response.StatusCode, response.Message);

            return Ok(response.Result);
        }

        // GET: odata/AdminCrudAccount/search?keyword=nam
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var response = await _service.SearchAsync(keyword);

            if (!response.IsSuccess)
                return StatusCode((int)response.StatusCode, response.Message);

            return Ok(response.Result);
        }
    }
}
