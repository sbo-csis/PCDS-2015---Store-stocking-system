namespace StoreStockingSystem.Models
{
    public class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SalesPerson SalesPerson { get; set; }
    }
}