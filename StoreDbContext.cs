using ConsoleApp1.Models;
using ConsoleApp1.Models.Base;
using ConsoleApp1.Models.Service;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public class StoreDbContext : DbContext
    {
        static StoreDbContext()
        {
            Database.SetInitializer(new StoreDatabaseInitializer());
        }

        // Service models
        public IDbSet<LastDocumentNumber> LastDocumentNumbers { get; set; }

        // Store models
        public IDbSet<Warehouse> Warehouses { get; set; }
        public IDbSet<Product> Products { get; set; }
        public IDbSet<PurchaseOrder> PurchaseOrders { get; set; }
        public IDbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; }

        public override int SaveChanges()
        {
            AddTimestamps();
            BeforeSaving();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync()
        {
            AddTimestamps();
            BeforeSaving();
            return base.SaveChangesAsync();
        }

        private void BeforeSaving()
        {
            var entities = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseDocument && (e.State == EntityState.Added || e.State == EntityState.Modified)).ToList();

            foreach (var entity in entities)
            {
                ((BaseDocument)entity.Entity).BeforeSaving();
            }
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(e => e.Entity is BaseEntity && (e.State == EntityState.Added || e.State == EntityState.Modified)).ToList();

            foreach (var entity in entities)
            {
                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).CreatedOn = DateTime.Now;
                }

                if (entity.State == EntityState.Modified)
                {
                    ((BaseEntity)entity.Entity).ModifiedOn = DateTime.Now;
                }
            }
        }

        private class StoreDatabaseInitializer : DropCreateDatabaseIfModelChanges<StoreDbContext>
        //private class StoreDatabaseInitializer : DropCreateDatabaseAlways<StoreDbContext>
        {
            protected override void Seed(StoreDbContext db)
            {
                base.Seed(db);

                var decimalFormatInfo = new NumberFormatInfo { NumberDecimalSeparator = "." };
                var seedData = Properties.Resources.MOCK_DATA.Split('\n')
                    .Skip(1).Where(line => !String.IsNullOrEmpty(line.Trim()))
                    .Select(line =>
                    {
                        var parts = line.Split(',');
                        return new SeedRow
                        {
                            Product = parts[0],
                            Qty = Decimal.Parse(parts[1].Trim(), decimalFormatInfo),
                            Price = Decimal.Parse(parts[2].Trim(), decimalFormatInfo),
                        };
                    }).ToList();

                using (var transaction = db.Database.BeginTransaction())
                {
                    var warehouse = new Warehouse() { Name = "Main store" }; db.Warehouses.Add(warehouse);
                    db.Warehouses.Add(new Warehouse() { Name = "Transfer" });

                    foreach (var row in seedData)
                    {
                        Product entity = new Product { Name = row.Product };
                        db.Products.Add(entity);
                        row.Entity = entity;
                    }

                    db.SaveChanges();

                    for (int i = 0; i < 100; i++)
                    {
                        var order = new PurchaseOrder()
                        {
                            WarehoseId = warehouse.Id,
                            Items = new List<PurchaseOrderItem>()
                        };

                        int rowCount = new Random().Next(1, seedData.Count);
                        for (int rowNum = 1; rowNum <= rowCount; rowNum++)
                        {
                            var item = seedData[rowNum];
                            order.Items.Add(new PurchaseOrderItem()
                            {
                                RowNum = rowNum,
                                ProductId = item.Entity.Id,
                                Qty = item.Qty,
                                Price = item.Price,
                                Total = item.Qty * item.Price,
                            });
                        }

                        db.PurchaseOrders.Add(order);
                    }

                    db.SaveChanges();
                    transaction.Commit();
                }
            }

            private class SeedRow
            {
                public string Product;
                public decimal Qty;
                public decimal Price;

                public Product Entity;
            }
        }
    }
}
