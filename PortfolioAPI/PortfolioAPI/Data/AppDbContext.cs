using Microsoft.EntityFrameworkCore;
using PortfolioAPI.Entities;

namespace PortfolioAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Project> Projects => Set<Project>();
    public DbSet<Blog> Blogs => Set<Blog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Project>(entity =>
        {
            entity.HasKey(project => project.Id);
            entity.Property(project => project.Title).IsRequired().HasMaxLength(200);
            entity.Property(project => project.Description).IsRequired().HasMaxLength(5000);
            entity.Property(project => project.Category).IsRequired().HasMaxLength(100);
            entity.Property(project => project.GitHubUrl).HasMaxLength(2048);
            entity.PrimitiveCollection(project => project.Technologies)
                .HasColumnType("nvarchar(max)").IsRequired();
        });

        modelBuilder.Entity<Blog>(entity =>
        {
            entity.HasKey(blog => blog.Id);
            entity.Property(blog => blog.Titel).IsRequired().HasMaxLength(200);
            entity.Property(blog => blog.Inhoud).IsRequired();
        });
    }
}
