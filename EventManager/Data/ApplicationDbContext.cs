using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EventManagementSystem.Models;

namespace EventManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<EventParticipant> EventParticipants { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<EventParticipant>()
                .HasKey(ep => new { ep.EventId, ep.UserId });

            builder.Entity<EventParticipant>()
                .HasOne(ep => ep.Event)
                .WithMany(e => e.Participants)
                .HasForeignKey(ep => ep.EventId);

            builder.Entity<EventParticipant>()
                .HasOne(ep => ep.User)
                .WithMany(u => u.AttendingEvents)
                .HasForeignKey(ep => ep.UserId);
        }
    }
}
