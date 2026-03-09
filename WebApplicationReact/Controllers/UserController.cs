using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplicationReact.Models.DTOs;
using WebApplicationReact.Models.Responses;
using WebApplicationReact.Services.Interfaces;

namespace WebApplicationReact.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        private readonly IUserService _userService = userService;

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<UserDetail>>>> GetUsers()
        {
            var result = await _userService.GetUsersAsync();

            if (!result.Success)
                return NotFound(result);

            return Ok(result);
        }
    }
}
