using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoreStockingSystem.Models
{
    public class Chain
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }

        public virtual List<Store> ChainStores { get; set; }
    }
}