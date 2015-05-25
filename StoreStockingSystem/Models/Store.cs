using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StoreStockingSystem.Models
{
    public class Store
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }
        public string Address { get; set; }
        public string PostalCode { get; set; }
        public string City { get; set; }
        public string ExternalId { get; set; }
        [ForeignKey("SalesPerson")]
        public int? SalesPersonId { get; set; }
        public virtual SalesPerson SalesPerson { get; set; }
        [ForeignKey("Chain")]
        public int? ChainId { get; set; }
        public virtual Chain Chain { get; set; }
        public int WarningPercentage { get; set; }
        //1 is highest
        [Range(1,3)]
        public int? StorePriority { get; set; }
    }
}