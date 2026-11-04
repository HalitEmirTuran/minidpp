using Microsoft.EntityFrameworkCore;

namespace MiniDpp.api.Data;

public class DppDbContext : DbContext
{
    public DppDbContext(DbContextOptions<DppDbContext> options) : base (options)
    {
        
    }

    public DbSet<ProductModel> ProductModels => Set<ProductModel>();
    public DbSet<Lot> Lots=> Set<Lot>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ProductModel>(e =>
        {e.Property(p => p.Gtin).HasMaxLength(14).IsRequired();
        
        e.HasIndex(p => p.Gtin).IsUnique();
    });

    modelBuilder.Entity<Lot>(e =>
        {e.HasOne(l => l.ProductModel)
        .WithMany(p => p.Lots)
        .HasForeignKey(l => l.ProductModelId)
        .OnDelete(DeleteBehavior.Cascade);
        e.HasIndex(l => new { l.ProductModelId, l.LotNum }).IsUnique();
    
    });
}}