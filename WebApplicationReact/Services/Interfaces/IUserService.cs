using WebApplicationReact.Models.DTOs;

namespace WebApplicationReact.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDetail>> GetUsersAsync();
    }
}
