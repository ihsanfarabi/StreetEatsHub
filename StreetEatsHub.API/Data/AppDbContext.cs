using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StreetEatsHub.API.Models;

namespace StreetEatsHub.API.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Vendor> Vendors { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Vendor entity
            modelBuilder.Entity<Vendor>(entity =>
            {
                entity.HasKey(v => v.Id);

                entity.Property(v => v.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(v => v.Location)
                    .IsRequired()
                    .HasMaxLength(200);

                entity.Property(v => v.Specialty)
                    .HasMaxLength(100);

                entity.Property(v => v.WhatsAppNumber)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(v => v.UserId)
                    .IsRequired();

                // Configure relationship with Identity User
                entity.HasOne(v => v.User)
                    .WithMany()
                    .HasForeignKey(v => v.UserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Configure relationship with MenuItems
                entity.HasMany(v => v.MenuItems)
                    .WithOne(m => m.Vendor)
                    .HasForeignKey(m => m.VendorId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Index for performance
                entity.HasIndex(v => v.UserId);
                entity.HasIndex(v => v.IsOpen);
            });

            // Configure MenuItem entity
            modelBuilder.Entity<MenuItem>(entity =>
            {
                entity.HasKey(m => m.Id);

                entity.Property(m => m.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(m => m.Price)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");

                entity.Property(m => m.Category)
                    .HasMaxLength(50)
                    .HasDefaultValue("General");

                entity.Property(m => m.IsAvailable)
                    .HasDefaultValue(true);

                // Index for performance
                entity.HasIndex(m => m.VendorId);
                entity.HasIndex(m => m.IsAvailable);
            });

            // Seed data for development (optional)
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // This will be useful for testing
            // You can add sample data here if needed
        }
    }
}