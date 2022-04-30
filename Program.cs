using ConsoleApp1.Models;
using ConsoleTables;
using System;
using System.Data.Entity;
using System.Linq;

namespace ConsoleApp1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ModifyOrder42();
            PrintOrder42();
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
