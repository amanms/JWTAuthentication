using Microsoft.EntityFrameworkCore;
using WebApplicationReact.Data;
using WebApplicationReact.Models.DTOs;
using WebApplicationReact.Models.Entities;
using WebApplicationReact.Repositories.Interfaces;

namespace WebApplicationReact.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(x => x.UserEmail == email);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<UserDetail>> GetUsersAsync(int pageNumber, int pageSize)
        {
            return await _context.Users
                .Select(u => new UserDetail
                {
                    UserId = u.UserId,
                    Name = u.UserName,
                    Email = u.UserEmail,
                    UserRole = u.UserRole
                })
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
    }
}
