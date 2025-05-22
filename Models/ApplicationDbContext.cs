using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Models.Models;

namespace Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public override DbSet<ApplicationUser> Users { get; set; }
        public DbSet<VacationSpot> VacationSpots { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.VacationSpot)
                .WithMany(v => v.Bookings)
                .HasForeignKey(b => b.SpotId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Payment)
                .WithOne(p => p.Booking)
                .HasForeignKey<Payment>(p => p.BookingId);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.VacationSpot)
                .WithMany(v => v.Reviews)
                .HasForeignKey(r => r.SpotId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<VacationSpot>()
                .HasOne(v => v.Owner)
                .WithMany(u => u.VacationSpot)
                .HasForeignKey(v => v.OwnerId);

            modelBuilder.Entity<VacationSpotCategory>()
                .HasKey(vsc => new { vsc.VacationSpotId, vsc.CategoryId });

            modelBuilder.Entity<VacationSpotCategory>()
                .HasOne(vsc => vsc.VacationSpot)
                .WithMany(vs => vs.VacationSpotCategories)
                .HasForeignKey(vsc => vsc.VacationSpotId);

            modelBuilder.Entity<VacationSpotCategory>()
                .HasOne(vsc => vsc.Category)
                .WithMany(c => c.VacationSpotCategories)
                .HasForeignKey(vsc => vsc.CategoryId);

        }
    }
}

