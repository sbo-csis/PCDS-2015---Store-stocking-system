using System.Data.Entity;

namespace StoreStockingSystem.Models
{
    public class StoreStockingContext : DbContext
    {
        public DbSet<Chain> Chains { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<DisplayType> DisplayTypes { get; set; }
        public DbSet<SalesPerson> SalesPersons { get; set; }
        public DbSet<ProductStock> ProductStocks { get; set; }

        public StoreStockingContext()
        {
            //TODO: Play with this a bit, does it ruin shit? This disables lazy loading, so not good per definition
            Configuration.ProxyCreationEnabled = false;
            Database.SetInitializer(new DropCreateDatabaseAlways<StoreStockingContext>()); //TODO: MUST NOT run in production. Enable check for environment, and disable if production.
        }
    }

}