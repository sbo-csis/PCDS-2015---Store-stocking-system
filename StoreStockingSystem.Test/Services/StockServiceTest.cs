using System;
using NUnit.Framework;
using StoreStockingSystem.Models;
using StoreStockingSystem.Services;

namespace StoreStockingSystem.Test.Services
{
    [TestFixture]
    public class StockServiceTest
    {
        [Test]
        public void can_add_product_to_stock()
        {
            using (var context = new StoreStockingContext())
            {

                var store = StoreService.AddStore(new Store
                {
                    Name = "Bilka Skagen (Unit Test)"
                }, context);

                var displayType = DisplayTypeService.AddDisplayType(new DisplayType
                {
                    Capacity = 10,
                    Name = "Pap-display (Unit Test)"
                }, context);

                var product = ProductService.NewProduct(new Product
                {
                    Name = "Pølsevogns massakre (Unit Test)",
                    Price = 299
                }, context);

                var stock = StockService.NewStock(store, displayType, null, 25, context);

                StockService.AddProductToStock(stock, product, 3, context);

                var productstock = StockService.GetProductStock(stock, product, context);

                if (productstock == null)
                    throw new Exception("No product stock found.");

                Assert.AreEqual(productstock.CurrentAmount, 3);
                Assert.AreEqual(productstock.ProductId, product.Id);
                Assert.AreEqual(productstock.StockId, stock.Id);
            }

        }

        [Test]
        public void can_remove_product_from_stock()
        {
            using (var context = new StoreStockingContext())
            {
                var store = StoreService.AddStore(new Store
                {
                    Name = "Bilka Skagen (Unit Test)"
                }, context);

                var displayType = DisplayTypeService.AddDisplayType(new DisplayType
                {
                    Capacity = 10,
                    Name = "Pap-display (Unit Test)"
                }, context);

                var product = ProductService.NewProduct(new Product
                {
                    Name = "Pølsevogns massakre (Unit Test)",
                    Price = 299
                }, context);

                var stock = StockService.NewStock(store, displayType, null, 25, context);

                StockService.AddProductToStock(stock, product, 3, context);

                var productstock = StockService.GetProductStock(stock, product, context);

                if (productstock == null)
                    throw new Exception("Could not create product stock.");

                Assert.AreEqual(productstock.CurrentAmount, 3);
                Assert.AreEqual(productstock.ProductId, product.Id);
                Assert.AreEqual(productstock.StockId, stock.Id);

                StockService.RemoveProductFromStock(stock, product, context);

                productstock = StockService.GetProductStock(stock, product, context);

                Assert.AreSame(productstock, null);
            }
        }

        [Test]
        public void get_warning_when_product_amount_below_warning_level()
        {
            using (var context = new StoreStockingContext())
            {
                var store = StoreService.AddStore(new Store
                {
                    Name = "Bilka Skagen (Unit Test)"
                }, context);

                var displayType = DisplayTypeService.AddDisplayType(new DisplayType
                {
                    Capacity = 10,
                    Name = "Pap-display (Unit Test)"
                }, context);

                var product = ProductService.NewProduct(new Product
                {
                    Name = "Pølsevogns massakre (Unit Test)",
                    Price = 299
                }, context);

                var stock = StockService.NewStock(store, displayType, null, 25, context);

                StockService.AddProductToStock(stock, product, 3, context);

                var warningStocks = StockService.GetLowStocks(context);

                Assert.AreEqual(warningStocks.Find(stock1 => stock1.Id == stock.Id), stock);
            }
        }

        [Test]
        public void get_no_warning_when_stock_product_amount_is_above_warning_level()
        {
            using (var context = new StoreStockingContext())
            {
                var store = StoreService.AddStore(new Store
                {
                    Name = "Bilka Skagen (Unit Test)"
                }, context);

                var displayType = DisplayTypeService.AddDisplayType(new DisplayType
                {
                    Capacity = 10,
                    Name = "Pap-display (Unit Test)"
                }, context);

                var product = ProductService.NewProduct(new Product
                {
                    Name = "Pølsevogns massakre (Unit Test)",
                    Price = 299
                }, context);

                var stock = StockService.NewStock(store, displayType, null, 1, context);

                StockService.AddProductToStock(stock, product, 3, context);

                var warningStocks = StockService.GetLowStocks(context);

                Assert.AreEqual(warningStocks.Find(stock1 => stock1.Id == stock.Id), null);
            }
        }
    }
}