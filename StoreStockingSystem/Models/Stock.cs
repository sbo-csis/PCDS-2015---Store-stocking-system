using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreStockingSystem.Models
{
    public class Stock
    {
        public Stock()
        {
            ProductStock = new List<ProductStock>();
        }

        [Key]
        public int Id { get; set; }

        [ForeignKey("Store")]
        [Column(Order = 1)]
        public int StoreId { get; set; }

        public virtual Store Store { get; set; }

        [ForeignKey("DisplayType")]
        [Column(Order = 2)]
        public int DisplayTypeId { get; set; }

        public virtual DisplayType DisplayType { get; set; }

        public int Capacity { get; set; } // Used in case of a store having 2 displays of the same type. Otherwise defaults to DisplayType capacity.

        public int WarningAmountLeft { get; set; }

        public virtual List<ProductStock> ProductStock { get; set; }
    }
}