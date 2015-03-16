using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace StoreStockingSystem.Models
{
    public class Sale : DbContext
    {
        [Key, ForeignKey("Store")]
        [Column(Order = 1)] 
        public int StoreId { get; set; }
        public virtual Store Store { get; set; }
        [Key, ForeignKey("Product")]
        [Column(Order = 2)] 
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public int SalesPrice { get; set; } // The price that a single product was sold for.
        [Key, ForeignKey("DisplayType")]
        [Column(Order = 3)] 
        public int DisplayTypeId { get; set; }
        public bool IsReturn { get; set; }
        public DisplayType DisplayType { get; set; }
        public DateTime SalesDate { get; set; }
    }
}