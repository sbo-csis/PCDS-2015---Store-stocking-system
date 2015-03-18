using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace StoreStockingSystem.Models
{
    public class ProductStock
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("Stock")]
        [Column(Order = 1)] 
        public int StockId { get; set; }
        public virtual Stock Stock { get; set; }
        [ForeignKey("Product")]
        [Column(Order = 2)] 
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public int Amount { get; set; }
    }
}