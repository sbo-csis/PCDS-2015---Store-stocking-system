using System.Collections.Generic;

namespace StoreStockingSystem.Models
{
    public class Stock
    {
        public int Id { get; set; }
        public Store Store { get; set; }
        public virtual DisplayType DisplayType { get; set; }
        public int Capacity { get; set; } // Used in case of a store having 2 displays of the same type. Otherwise defaults to DisplayType capacity.
        public int WarningPercentageStockLeft { get; set; }
        public virtual List<ProductStock> ProductStock { get; set; }
    }
}