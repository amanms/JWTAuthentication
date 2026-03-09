using WebApplicationReact.Models.DTOs;
using WebApplicationReact.Models.Responses;

namespace WebApplicationReact.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApiResponse<List<UserDetail>>> GetUsersAsync();
    }
}
