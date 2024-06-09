using DTOS;
using Microsoft.AspNetCore.Mvc;
using NguyenMinhNguyen_Assignment2.MessageStatusResponse;
using Service.Interface;

namespace NguyenMinhNguyen_Assignment2.Controllers
{
    [Route("api/")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ISystemAccountService _systemAccountService;

        public LoginController(ISystemAccountService systemAccountService)
        {
            _systemAccountService = systemAccountService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var account = await _systemAccountService.LoginAccount(loginDto);
            if(account != null)
            {
                return Ok(account);
            }
            else
            {
                return Unauthorized(new ApiResponseStatus(401));
            }
        }
    }
}
