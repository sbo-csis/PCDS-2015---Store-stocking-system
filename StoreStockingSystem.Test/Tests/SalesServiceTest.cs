using System;
using NUnit.Framework;
using StoreStockingSystem.Models;
using StoreStockingSystem.Services;

namespace StoreStockingSystem.Test.Tests
{
    [TestFixture]
    public class SalesServiceTest
    {
        [Test]
        public void make_sale_and_check_that_stock_goes_down_by_1()
        {
            using (var context = new StoreStockingContext())
            {
                var store = StoreService.AddStore(new Store()
                {
                    Name = "Bilka Skagen (Unit Test)"
                }, context);

                var displayType = DisplayTypeService.AddDisplayType(new DisplayType()
                {
                    Capacity = 10,
                    Name = "Plastik display (Unit Test)"
                }, context);

                var product = ProductService.NewProduct(new Product()
                {
                    Name = "Kør sportsvogn (Unit Test)",
                    Price = 199
                }, context);

                var stock = StockService.NewStock(store, displayType, null, 25, context);

                StockService.AddProductToStock(stock.Id, product.Id, 3, context);

                SalesService.RegisterSale(new Sale()
                {
                    Store = store,
                    Product = product,
                    SalesPrice = 199,
                    DisplayType = displayType,
                    SalesDate = DateTime.Now
                }, context);

                var productstock = StockService.GetProductStock(stock, product, context);

                if (productstock == null)
                    throw new Exception("No product stock found.");

                Assert.AreEqual(2, productstock.Amount);
            }
        }

        [Test]
        public void make_return_sale_and_check_that_stock_goes_up_by_1()
        {
            using (var context = new StoreStockingContext())
            {
                var store = StoreService.AddStore(new Store()
                {
                    Name = "Bilka Skagen (Unit Test)"
                }, context);

                var displayType = DisplayTypeService.AddDisplayType(new DisplayType()
                {
                    Capacity = 10,
                    Name = "Plastik display (Unit Test)"
                }, context);

                var product = ProductService.NewProduct(new Product()
                {
                    Name = "Kør sportsvogn (Unit Test)",
                    Price = 199
                }, context);

                var stock = StockService.NewStock(store, displayType, null, 25, context);

                StockService.AddProductToStock(stock.Id, product.Id, 3, context);

                SalesService.RegisterSale(new Sale()
                {
                    Store = store,
                    Product = product,
                    SalesPrice = 199,
                    DisplayType = displayType,
                    SalesDate = DateTime.Now,
                    IsReturn = true
                }, context);

                var productstock = StockService.GetProductStock(stock, product, context);

                if (productstock == null)
                    throw new Exception("No product stock found.");

                Assert.AreEqual(4, productstock.Amount);
            }
        }
    }
}