using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace GymManagementSystem.DAL
{
    public class GymDbContext : DbContext
    {
        public GymDbContext(DbContextOptions<GymDbContext> options) : base(options) 
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=.; Database=GymManagementSystemDB; Trusted_Connection=True; TrustServerCertificate=True; ");
        //}

        public DbSet<Plan> Plans { get; set; }

    }
}
