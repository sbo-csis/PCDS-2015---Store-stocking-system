using System;
using NUnit.Framework;
using StoreStockingSystem.Models;
using StoreStockingSystem.Services;

namespace StoreStockingSystem.Test.Tests
{
    [TestFixture]
    public class SalesServiceTest
    {
        [Ignore]
        [Test]
        public void make_sale_and_check_stock_gets_modified()
        {
            var salesperson = SalesPersonService.AddSalesPerson(new SalesPerson()
            {
                Name = "Bob"
            });

            var store = StoreService.AddStore(new Store()
            {
                Name = "Bilka Skagen (Unit Test)",
                SalesPerson = salesperson
            });

            var displayType = DisplayTypeService.AddDisplayType(new DisplayType()
            {
                Capacity = 10,
                Name = "Plastik display (Unit Test)"
            });

            var product = ProductService.NewProduct(new Product()
            {
                Name = "Kør sportsvogn (Unit Test)",
                Price = 199
            });

            var stock = StockService.NewStock(store, displayType, null, 25);

            StockService.AddProductToStock(stock, product, 3);

            SalesService.RegisterSale(new Sale()
            {
                Store = store,
                Product = product,
                SalesPrice = 199,
                DisplayType = displayType,
                SalesDate = DateTime.Now
            });

            var productstock = StockService.GetProductStock(stock, product);

            if (productstock == null)
                throw new Exception("No product stock found.");

            Assert.AreEqual(2, productstock.Amount);
        }
    }
}