using Common.Dto;
using DLL.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_ASSI1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminCrudAccountController : ControllerBase
    {
        private readonly IAdminCrudAccountService _service;

        public AdminCrudAccountController(IAdminCrudAccountService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _service.GetAllAsync();
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(short id)
        {
            var response = await _service.GetByIdAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdminCRUDdto dto)
        {


            var response = await _service.CreateAsync(dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(short id, [FromBody] AdminCRUDdto dto)
        {
            var response = await _service.UpdateAsync(id, dto);
            return StatusCode(response.StatusCode, response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(short id)
        {
            var response = await _service.DeleteAsync(id);
            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string keyword)
        {
            var response = await _service.SearchAsync(keyword);

            if (!response.IsSuccess)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }
    }
}
