using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Castle.DynamicLinqQueryBuilder.Tests31.Database
{
    public class StoreContext: DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options)
            : base(options)
        {
        }

        public DbSet<Store> Stores { get; set; }
        public DbSet<Product> Products { get; set; }
    }

    public class StoreContextFactory : IDesignTimeDbContextFactory<StoreContext>
    {
        public StoreContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StoreContext>();
            optionsBuilder.UseSqlite("Filename=:memory:");

            return new StoreContext(optionsBuilder.Options);
        }
    }

    public class Store
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid StoreId { get; set; }

        public string StoreName { get; set; }
        public DateTime OpenDate { get; set; }
        public double TotalRevenue { get; set; }

        public virtual List<Product> Products { get; set; }
    }

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
