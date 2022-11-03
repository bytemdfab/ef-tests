using ConsoleApp1.Models;
using ConsoleTables;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ModifyOrder42();
            PrintOrder42();

            PrintLiquors("Wine", "Vodka", "Whiskey");
        }

        private static void PrintLiquors(params string[] names)
        {
            // "p" parameter in any {p => ...} lambda
            var paramProduct = Expression.Parameter(typeof(Product), "p");

            // "p.Name" in any {p => p.Name ...} lambda
            var propName = Expression.Property(paramProduct, nameof(Product.Name));

            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) })!;

            // Initial expression in sequence. As we define "or" sequence - first expression will be {p => false}
            var filterExpression = Expression.Lambda<Func<Product, bool>>(Expression.Constant(false), paramProduct);

            foreach (var name in names)
            {
                // Lambda {p => p.Name.Contains("<name>")}
                var nameContainsExpr = Expression.Lambda<Func<Product, bool>>(Expression.Call(propName, containsMethod, Expression.Constant(name)), paramProduct);

                // Append lambda to accumulated expression using or operator
                filterExpression = Expression.Lambda<Func<Product, bool>>(Expression.Or(filterExpression.Body, nameContainsExpr.Body), paramProduct);
            }

            Console.WriteLine($"Filter expression: {filterExpression.Body.ToString()}");

            using (StoreDbContext db = new StoreDbContext())
            {
                db.Database.Log = s => Console.WriteLine(s);

                db.Products.AsNoTracking()
                    .Where(filterExpression)
                    .OrderBy(product => product.Name)
                    .ToList()
                    .ForEach(product => Console.WriteLine(product.Name));
            }
        }

        private static void ModifyOrder42()
        {
            using (StoreDbContext db = new StoreDbContext())
            {
                var order = db.PurchaseOrders.Where(x => x.Number == "ST-000042").FirstOrDefault();
                var newWarehouse = db.Warehouses.Where(x => x.Name == "Transfer").FirstOrDefault();

                if (order != null && newWarehouse != null)
                {
                    order.WarehoseId = newWarehouse.Id;
                }

                db.SaveChanges();
            }
        }

        private static void PrintOrder42()
        {
            string numToFind = "ST-000042";
            using (StoreDbContext db = new StoreDbContext())
            {
                var order = db.PurchaseOrders
                    .Include(o => o.Warehouse)
                    .Include(o => o.Items.Select(i => i.Product))
                    .Where(x => x.Number == numToFind)
                    .FirstOrDefault();

                if (order != null)
                {
                    Console.WriteLine($"No.         : {order.Number}");
                    Console.WriteLine($"Date        : {order.Date}");
                    Console.WriteLine($"Warehouse   : {order.Date}");
                    Console.WriteLine($"-------------");
                    Console.WriteLine($"Created on  : {order.Date}");
                    Console.WriteLine($"Modified on : {order.Date}");
                    Console.WriteLine($"-------------");
                    Console.WriteLine($"Items:");

                    var table = new ConsoleTable("N", "Product", "Qty", "Price", "Total");

                    foreach (var item in order.Items)
                    {
                        table.AddRow(
                            item.RowNum, 
                            item.Product.Name, 
                            item.Qty, 
                            item.Price, 
                            item.Total);
                    }
                    table.Write(Format.Alternative);
                }
                else
                    Console.WriteLine($"Document {numToFind} not found :(");
            }
        }
    }
}
