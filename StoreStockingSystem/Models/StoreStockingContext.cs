using System.Data.Entity;
using StoreStockingSystem.Data;

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
            Configuration.ProxyCreationEnabled = false; // Currently disables lazy loading to avoid includes all over the place.

            if (Database.Exists())
            {
                Database.SetInitializer(new RecreateDbIfModelChangesAndInsertSeedData()); //TODO: MUST NOT run in production. Enable check for environment, and disable if production.
            }
            else
            {
                Database.SetInitializer(new BuildDatabaseFromScratch()); //TODO: MUST NOT run in production. Enable check for environment, and disable if production.
            }

        }

        private class RecreateDbIfModelChangesAndInsertSeedData : DropCreateDatabaseIfModelChanges<StoreStockingContext>
        {
            protected override void Seed(StoreStockingContext context)
            {
                TestData.BuildData(context);
            }
        }

        private class BuildDatabaseFromScratch : CreateDatabaseIfNotExists<StoreStockingContext>
        {
            protected override void Seed(StoreStockingContext context)
            {
                TestData.BuildData(context);
            }
        }

    }

}