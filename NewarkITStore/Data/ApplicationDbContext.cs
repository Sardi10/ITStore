﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NewarkITStore.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }

    public DbSet<ProductType> ProductTypes { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Set precision for price fields
        modelBuilder.Entity<Product>()
            .Property(p => p.RecommendedPrice)
            .HasPrecision(18, 2); // Decimal(18,2)

        modelBuilder.Entity<BasketItem>()
            .Property(b => b.PricePerUnit)
            .HasPrecision(18, 2);
    }
}
