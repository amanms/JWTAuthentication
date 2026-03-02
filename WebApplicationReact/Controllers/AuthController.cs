using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplicationReact.Helpers;
using WebApplicationReact.Models.DTOs;
using WebApplicationReact.Services.Interfaces;

namespace WebApplicationReact.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        

        public AuthController(IAuthService authService)
        {
            _authService = authService;

        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            try
            {
                await _authService.RegisterAsync(request);

                return StatusCode(201, new
                {
                    success = true,
                    message = "User registered successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest request)
        {
            var response = await _authService.LoginAsync(request);

            Response.Cookies.Append("jwtToken", response.Token, new CookieOptions
            {
                HttpOnly = true,          
                Secure = true,            
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            return Ok(new { role = response.Role });
        }
    }
}
