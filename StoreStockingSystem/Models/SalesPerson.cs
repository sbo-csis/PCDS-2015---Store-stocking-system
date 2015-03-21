using System.ComponentModel.DataAnnotations;

namespace StoreStockingSystem.Models
{
    public class SalesPerson
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}