using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        // "Users" is the name of the table
        public DbSet<AppUser> Users { get; set; }
    }
}