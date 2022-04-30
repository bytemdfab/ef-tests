using ConsoleApp1.Models.Base;
using System;
using System.Collections.Generic;

namespace ConsoleApp1.Models
{
    public class PurchaseOrder : BaseDocument
    {
        public Guid WarehoseId { get; set; }
        public Warehouse Warehouse { get; set; }

        public ICollection<PurchaseOrderItem> Items { get; set; }
    }
}
