using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
using StoreStockingSystem.Models;
using StoreStockingSystem.Services;

namespace StoreStockingSystem.Test.Tests
{
    [TestFixture]
    public class StockServiceTest
    {
        [Ignore]
        [TestAttribute]
        public void can_add_product_to_stock()
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

            var productId = ProductService.NewProduct(new Product()
            {
                Name = "Pølsevogns massakre (Unit Test)",
                Price = 299
            });

            var stockId = StockService.NewStock(storeId, displayId, null, 25);

            StockService.AddProductToStock(stockId, productId, 3);

            var productstock = StockService.GetProductStock(stockId, productId);

            if(productstock == null)
                throw new Exception("No product stock found.");

            Assert.AreEqual(productstock.Amount, 3);
            Assert.AreEqual(productstock.ProductId, productId);
            Assert.AreEqual(productstock.StockId, stockId);
        }

        [Ignore]
        [TestAttribute]
        public void can_remove_product_to_stock()
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

            var productId = ProductService.NewProduct(new Product()
            {
                Name = "Pølsevogns massakre (Unit Test)",
                Price = 299
            });

            var stockId = StockService.NewStock(storeId, displayId, null, 25);

            StockService.AddProductToStock(stockId, productId, 3);
            
            var productstock = StockService.GetProductStock(stockId, productId);

            if (productstock == null)
                throw new Exception("No product stock found.");

            Assert.AreEqual(productstock.Amount, 3);
            Assert.AreEqual(productstock.ProductId, productId);
            Assert.AreEqual(productstock.StockId, stockId);

            StockService.RemoveProductFromStock(stockId, productId);

            productstock = StockService.GetProductStock(stockId, productId);

            Assert.AreSame(productstock, null);
        }
    }
}