using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Castle.DynamicLinqQueryBuilder.Tests.Database;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;

namespace Castle.DynamicLinqQueryBuilder.Tests.ORM
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class ORMTests: IDisposable
    {
        private readonly SqliteConnection _connection;
        private readonly DbContextOptions<StoreContext> _contextOptions;

        public ORMTests()
        {
            _connection = new SqliteConnection("Filename=:memory:");
            _connection.Open();

            _contextOptions = new DbContextOptionsBuilder<StoreContext>()
                .UseSqlite(_connection)
                .Options;

            // Create the schema and seed some data
            using var context = new StoreContext(_contextOptions);

            context.Database.EnsureCreated();


            var stores = new List<Store>()
            {
                new Store()
                {
                    StoreId = Guid.NewGuid(),
                    OpenDate = new DateTime(2020, 12, 18),
                    StoreName = "Tractor Store",
                    TotalRevenue = 200000000,
                    Products = new List<Product>()
                    {
                        new Product()
                        {
                            ProductName = "Model 12 Tractor",
                            ProductPrice = 75000
                        },
                        new Product()
                        {
                            ProductName = "Model 14 Tractor",
                            ProductPrice = 122000
                        },
                        new Product()
                        {
                            ProductName = "Plow",
                            ProductPrice = 1200
                        }
                    }
                },
                new Store()
                {
                    StoreId = Guid.NewGuid(),
                    OpenDate = new DateTime(2021, 12, 15),
                    StoreName = "Tire Store",
                    TotalRevenue = 150000000,
                    Products = new List<Product>()
                    {
                        new Product()
                        {
                            ProductName = "Tractor Tire",
                            ProductPrice = 12000
                        },
                        new Product()
                        {
                            ProductName = "Car Tire",
                            ProductPrice = 120
                        },
                        new Product()
                        {
                            ProductName = "Cap",
                            ProductPrice = 75
                        }
                    }
                }
            };
        
            context.AddRange(stores);
            context.SaveChanges();

        }

        StoreContext CreateContext() => new StoreContext(_contextOptions);
        
        public void Dispose() => _connection.Dispose();

        [Test]
        public void ContainsTractorTest()
        {
            var context = CreateContext();

            //expect 3 entries to match for an contains Tractor comparison
            var tractorFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "ProductName",
                        Id = "ProductName",
                        Input = "NA",
                        Operator = "contains",
                        Type = "string",
                        Value = new[] { "Tractor" }
                    }
                }
            };

            var tractorIdFilteredList = context.Products.BuildQuery(tractorFilter).ToList();
            Assert.IsTrue(tractorIdFilteredList != null);
            Assert.IsTrue(tractorIdFilteredList.Count == 3);
        }

        [Test]
        public void BeginsWithModelTest()
        {
            var context = CreateContext();

            //expect 2 entries to match for a begins with comparison
            var modelFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "ProductName",
                        Id = "ProductName",
                        Input = "NA",
                        Operator = "begins_with",
                        Type = "string",
                        Value = new[] { "Model" }
                    }
                }
            };

            var beginsFilteredList = context.Products.BuildQuery(modelFilter).ToList();
            Assert.IsTrue(beginsFilteredList != null);
            Assert.IsTrue(beginsFilteredList.Count == 2);
        }

        [Test]
        public void EqualsTireStoreTest()
        {
            var context = CreateContext();

            //expect 1 entry to match for an contains Tire Store comparison
            var storeFilter = new QueryBuilderFilterRule
            {
                Condition = "and",
                Rules = new List<QueryBuilderFilterRule>
                {
                    new QueryBuilderFilterRule
                    {
                        Condition = "and",
                        Field = "StoreName",
                        Id = "StoreName",
                        Input = "NA",
                        Operator = "equal",
                        Type = "string",
                        Value = new[] { "Tire Store" }
                    }
                }
            };

            var storeFilteredList = context.Stores.BuildQuery(storeFilter).ToList();
            Assert.IsTrue(storeFilteredList != null);
            Assert.IsTrue(storeFilteredList.Count == 1);
        }


    }
}
