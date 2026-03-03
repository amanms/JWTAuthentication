using Microsoft.EntityFrameworkCore;
using WebApplicationReact.Models.Entities;

namespace WebApplicationReact.Data
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
    }
}
