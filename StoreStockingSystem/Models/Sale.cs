using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;

namespace StoreStockingSystem.Models
{
    public class Sale : DbContext
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Store")]
        [Column(Order = 1)] 
        public int StoreId { get; set; }
        public virtual Store Store { get; set; }
        [ForeignKey("Product")]
        [Column(Order = 2)] 
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        [ForeignKey("DisplayType")]
        [Column(Order = 3)]
        public int DisplayTypeId { get; set; }
        public virtual DisplayType DisplayType { get; set; }

        public int SalesPrice { get; set; } // The price that a single product was sold for.
        public bool IsReturn { get; set; }
        public DateTime SalesDate { get; set; }
    }
}