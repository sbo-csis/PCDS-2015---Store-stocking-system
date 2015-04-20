using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StoreStockingSystem.Models
{
    public class Chain
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
<<<<<<< HEAD
        public string ExternalId { get; set; }
=======

        public virtual List<Store> ChainStores { get; set; }
>>>>>>> ae36a382139fe1143680a2912c99c9c3e1553486
    }
}