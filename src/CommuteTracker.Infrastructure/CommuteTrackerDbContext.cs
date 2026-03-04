//create database context for commute tracker application
using CommuteTracker.Core.Entities;
using Microsoft.EntityFrameworkCore;    
namespace CommuteTracker.Infrastructure
{
    public class CommuteTrackerDbContext : DbContext
    {
        public CommuteTrackerDbContext(DbContextOptions<CommuteTrackerDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<LocationPoint> LocationPoints { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>()
                .HasMany(u => u.Trips)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);
            modelBuilder.Entity<Trip>()
                .HasMany(t => t.LocationPoints)
                .WithOne(lp => lp.Trip)
                .HasForeignKey(lp => lp.TripId);
        }
    }
}