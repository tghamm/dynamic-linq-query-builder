using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Castle.DynamicLinqQueryBuilder.Tests.Database
{
    [ExcludeFromCodeCoverage]
    public class StoreContext: DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options)
            : base(options)
        {
        }

        public DbSet<Store> Stores { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<StoreOwner> StoreOwners { get; set; }
    }
    [ExcludeFromCodeCoverage]
    public class StoreContextFactory : IDesignTimeDbContextFactory<StoreContext>
    {
        public StoreContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StoreContext>();
            optionsBuilder.UseSqlite("Filename=:memory:");

            return new StoreContext(optionsBuilder.Options);
        }
    }
    [ExcludeFromCodeCoverage]
    public class Store
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid StoreId { get; set; }

        public string StoreName { get; set; }
        public DateTime OpenDate { get; set; }
        public double TotalRevenue { get; set; }

        public virtual List<Product> Products { get; set; }
        public virtual  StoreOwner StoreOwner { get; set; }
    }

    public class StoreOwner
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StoreOwnerId { get; set; }
        public string StoreOwnerName { get; set; }
        [ForeignKey("Store")]
        public Guid StoreId { get; set; }

        public virtual Store Store { get; set; }
    }
    [ExcludeFromCodeCoverage]
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; }
        [ForeignKey("Store")]
        public Guid StoreId { get; set; }

        public virtual Store Store { get; set; }

        public string ProductName { get; set; }
        public double ProductPrice { get; set; }

    }
}
