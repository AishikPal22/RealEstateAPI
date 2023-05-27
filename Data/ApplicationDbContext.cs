using Microsoft.EntityFrameworkCore;
using RealEstateApplication.Models;

namespace RealEstateApplication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Property> Properties { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=MyNotebook;Database=RealEstateDb;Trusted_Connection=True;Encrypt=False;");
        }
    }
}
