using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Define DbSet properties for your entities
        public DbSet<Event> Event { get; set; }
        public DbSet<EventRegistration> EventRegistration { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Apply configurations from the same assembly
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}