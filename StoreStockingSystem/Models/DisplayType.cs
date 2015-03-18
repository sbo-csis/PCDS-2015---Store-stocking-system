using System.ComponentModel.DataAnnotations;

namespace StoreStockingSystem.Models
{
    public class DisplayType
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
    }
}