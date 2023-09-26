using Cerber.Api.Database.Model;
using Microsoft.EntityFrameworkCore;

namespace Cerber.Api.Database.EntityFramework;

public class CerberDbContext : DbContext
{
    public DbSet<HeartbeatEntity> Heartbeats { get; set; } = null!;
    public DbSet<ProductEntity> Products { get; set; } = null!;

    public CerberDbContext(DbContextOptions<CerberDbContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var product = modelBuilder.Entity<ProductEntity>().ToTable("product");
        product.HasKey(p => p.Id).HasName("pk_product");
        product.Property(h => h.Id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
        product.Property(p => p.Name).HasColumnName("name").HasMaxLength(50).IsRequired();
        product.Property(p => p.Description).HasColumnName("description").HasMaxLength(200);
        product.HasIndex(p => new { p.Name }).HasDatabaseName("ix_product_name").IsUnique();
        product.HasMany<HeartbeatEntity>(p => p.Heartbeats);

        var model = modelBuilder.Entity<HeartbeatEntity>().ToTable("heartbeat");
        model.HasKey(h => h.Id).HasName("pk_heartbeat");
        model.Property(h => h.Id).HasColumnName("id").IsRequired().ValueGeneratedOnAdd();
        model.Property(h => h.ClientTimestamp).HasColumnName("client_timestamp");
        model.Property(h => h.ServerTimestamp).HasColumnName("server_timestamp").IsRequired();
        model.Property(h => h.ProductId).HasColumnName("product_id").IsRequired();
        model.Property(h => h.Version).HasColumnName("version").HasMaxLength(200);
        model.Property(h => h.Instance).HasColumnName("instance").HasMaxLength(200);
        model.HasIndex(h => new { h.ProductId, h.Instance, h.Version })
            .HasDatabaseName("ix_heartbeat_product_instance_version");
    }
}