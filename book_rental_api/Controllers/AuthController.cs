using Microsoft.AspNetCore.Mvc;  
using book_rental_api.Services;
using book_rental_api.Models;
using book_rental_api.Controllers;

namespace server.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : APIControllerBase
    {

        private readonly IAuthService _auth;

        public AuthController(IAuthService auth, IHttpContextAccessor httpContextAccessor): base(httpContextAccessor)
        {

            _auth = auth;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDTO loginUserDTO)
        {
            return Ok(await _auth.Login(loginUserDTO));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO registerUserDTO)
        {
            return Ok(await _auth.Register(registerUserDTO));
        }
    }
}
