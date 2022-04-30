using ConsoleApp1.Models.Base;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConsoleApp1.Models
{
    public class PurchaseOrderItem : BaseEntity
    {
        [ForeignKey("Parent")]
        public Guid ParentId { get; set; }
        public PurchaseOrder Parent { get; set; }

        public int RowNum { get; set; }

        public Guid ProductId { get; set; }
        public Product Product { get; set; }

        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public decimal Total { get; set; }
    }
}
