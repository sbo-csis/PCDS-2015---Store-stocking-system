using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Runtime.Remoting.Proxies;

namespace StoreStockingSystem.Models
{
    public class StoreStockingContext : DbContext
    {
        public DbSet<Store> Stores { get; set; }
        public DbSet<Stock> Stocks { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<DisplayType> DisplayTypes { get; set; }
        public DbSet<SalesPerson> SalesPersons { get; set; }
        public DbSet<ProductStock> ProductStocks { get; set; }

        public StoreStockingContext()
        {
            Configuration.ProxyCreationEnabled = false;          
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<StoreStockingContext>()); //TODO: MUST NOT run in production. Enable check for environment, and disable if production.
        }
    }

}