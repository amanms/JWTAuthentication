using Microsoft.EntityFrameworkCore;
using WebApplicationReact.Models.Entities;

namespace WebApplicationReact.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task AddAsync(User user);

        Task<List<User>> GetAllAsync();
    }
}
