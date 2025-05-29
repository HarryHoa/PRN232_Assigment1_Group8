using Common.Dto;
using DLL.Interface;
using DLL.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace PRN232_ASSI1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemAccountController : ControllerBase
    {
        private readonly ISystemAccountService _systemAccountServices;

       
        public SystemAccountController(ISystemAccountService systemAccountServices)
        {
            _systemAccountServices = systemAccountServices;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _systemAccountServices.Login(loginDto.Email, loginDto.Password);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser([FromBody] SystemAccountDto account)
        {
           
            var response = await _systemAccountServices.ResgisterUser(account);
            if (response.IsSuccess)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
