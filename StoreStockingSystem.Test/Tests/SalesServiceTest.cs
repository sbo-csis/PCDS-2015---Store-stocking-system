using NUnit.Framework;
using StoreStockingSystem.Models;
using StoreStockingSystem.Services;

namespace StoreStockingSystem.Test.Tests
{
    [TestFixture]
    public class SalesServiceTest
    {
        [Test]
        public void make_sale_and_check_stock_gets_modified()
        {

            var storeId = StoreService.AddStore(new Store()
            {
                Name = "Bilka Skagen (Unit Test)"
            });

            var displayId = DisplayTypeService.AddDisplayType(new DisplayType()
            {
                Capacity = 10,
                Name = "Pap-display (Unit Test)"
            });

            //StockService.AddStock();

            // TODO: Make sales test as soon as store, product and displaytype is up and running.
        }
    }
}