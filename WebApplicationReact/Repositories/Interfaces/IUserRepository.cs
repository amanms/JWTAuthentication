using Microsoft.EntityFrameworkCore;
using WebApplicationReact.Models.Entities;
using WebApplicationReact.Models.DTOs;

namespace WebApplicationReact.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task AddAsync(User user);

        Task<List<UserDetail>> GetAllAsync();
    }
}
