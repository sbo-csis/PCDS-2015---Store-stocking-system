namespace StoreStockingSystem.Models
{
    public class Store
    {
        public int Id { get; set; }
        public virtual string Name { get; set; }
        public SalesPerson SalesPerson { get; set; }
    }
}