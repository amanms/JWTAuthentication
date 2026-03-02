using Microsoft.EntityFrameworkCore;
using WebApplicationReact.Models.Entities;

namespace WebApplicationReact.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}
