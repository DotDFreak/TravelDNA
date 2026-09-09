using Microsoft.EntityFrameworkCore;
using TravelDNA.Core.Models;

namespace TravelDNA.Infrastructure.Data;

public sealed class TravelDnaDbContext(DbContextOptions<TravelDnaDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("users");
            entity.HasKey(user => user.Id);
            entity.Property(user => user.Email).HasMaxLength(320).IsRequired();
            entity.Property(user => user.DisplayName).HasMaxLength(200);
            entity.Property(user => user.GoogleSubjectId).HasMaxLength(255);
            entity.Property(user => user.AuthProvider).HasMaxLength(50).IsRequired();
            entity.Property(user => user.CreatedAtUtc).HasColumnName("created_at").IsRequired();
            entity.Property(user => user.UpdatedAtUtc).HasColumnName("updated_at").IsRequired();
            entity.HasIndex(user => user.Email).IsUnique();
            entity.HasIndex(user => user.GoogleSubjectId).IsUnique();
        });
    }
}