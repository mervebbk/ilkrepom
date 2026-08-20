using IlkRepom.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace IlkRepom.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Username)
                      .IsRequired()
                      .HasMaxLength(50);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(256);

                entity.Property(u => u.PasswordHash)
                      .IsRequired();

                entity.HasIndex(u => u.Username).IsUnique();
                entity.HasIndex(u => u.Email).IsUnique();
            });
        }
    }
}