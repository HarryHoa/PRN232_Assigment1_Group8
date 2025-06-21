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
        [HttpGet("/api/GetAccounts")]
        public async Task<IActionResult> Get()
        {
            var response = await _service.GetAllAsync();

            if (!response.IsSuccess)
                return StatusCode((int)response.StatusCode, response.Message);

            return Ok(response.Result); // thường là danh sách IQueryable<AdminCRUDdto>
        }
       
        [HttpGet("/api/GetAccountsById/{id}")]
        public async Task<IActionResult> GetById(short id)
        {
            var response = await _service.GetByIdAsync(id);

            if (!response.IsSuccess)
                return StatusCode((int)response.StatusCode, response.Message);

            return Ok(response.Result);
        }
        [EnableQuery]
        [HttpPost("/api/CreateAccounts")]
        public async Task<IActionResult> Create([FromBody] AdminCRUDdto dto)
        {
            var response = await _service.CreateAsync(dto);

            if (!response.IsSuccess)
                return StatusCode((int)response.StatusCode, response.Message);

            return Ok(response.Result);
        }

        [HttpPut("/api/UpdateAccounts/{id}")]
        public async Task<IActionResult> Update(short id, [FromBody] AdminCRUDdto dto)
        {
            var response = await _service.UpdateAsync(id, dto);

            if (!response.IsSuccess)
                return StatusCode((int)response.StatusCode, response.Message);

            return Ok(response.Result);
        }

        [HttpDelete("/api/DeleteAccounts/{id}")]
        public async Task<IActionResult> Delete(short id)
        {
            var response = await _service.DeleteAsync(id);

            if (!response.IsSuccess)
                return StatusCode((int)response.StatusCode, response.Message);

            return Ok(response.Result);
        }

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
