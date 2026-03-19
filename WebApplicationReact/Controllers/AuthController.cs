using Microsoft.AspNetCore.Authorization;
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
        public async Task<ActionResult<ApiResponse<object>>> Register([FromForm] RegisterRequest request)
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

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = result.Message,
                Data = new
                {
                    token = result.Data.Token,
                    refreshToken = result.Data.RefreshToken,
                    role = result.Data.Role
                }
            });
        }

        [HttpPost("logout")]
        public async Task<ActionResult<ApiResponse<object>>> Logout([FromHeader(Name = "x-refresh-token")] string refreshToken)
        {
            
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _authService.RevokeTokenAsync(refreshToken);
            }

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Logged out successfully",
                Data = null
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromHeader(Name = "x-refresh-token")] string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Refresh token is missing"
                });

            var result = await _authService.RefreshTokenAsync(refreshToken);

            if (!result.Success)
                return Unauthorized(result);

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = result.Message,
                Data = new
                {
                    token = result.Data.Token,
                    refreshToken = result.Data.RefreshToken
                }
            });
        }
    }
}
