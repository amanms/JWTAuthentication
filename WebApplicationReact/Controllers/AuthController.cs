using Microsoft.AspNetCore.Mvc;
using WebApplicationReact.Models.DTOs;
using WebApplicationReact.Models.Responses;
using WebApplicationReact.Services.Interfaces;

namespace WebApplicationReact.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        private readonly IAuthService _authService = authService;

        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<object>>> Register(RegisterRequest request)
        {
            var result = await _authService.RegisterAsync(request);

            if (!result.Success)
                return BadRequest(result);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<object>>> Login(LoginRequest request)
        {
            var result = await _authService.LoginAsync(request);

            if (!result.Success)
                return Unauthorized(result);

            Response.Cookies.Append("jwtToken", result.Data.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTime.UtcNow.AddHours(1)
            });

            var response = new ApiResponse<object>
            {
                Success = true,
                Message = result.Message,
                Data = new { role = result.Data.Role }
            };

            return Ok(response);
        }

        [HttpPost("logout")]
        public ActionResult<ApiResponse<object>> Logout()
        {
            Response.Cookies.Delete("jwtToken", new CookieOptions
            {
                Secure = true,
                SameSite = SameSiteMode.None
            });

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Logged out successfully",
                Data = null
            });
        }
    }
}
