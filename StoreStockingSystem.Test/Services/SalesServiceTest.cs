using System;
using System.Linq;
using NUnit.Framework;
using StoreStockingSystem.Models;
using StoreStockingSystem.Services;

namespace StoreStockingSystem.Test.Services
{
    [TestFixture]
    public class SalesServiceTest
    {
        [Test]
        public void make_sale_and_check_that_stock_goes_down_by_1()
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
                    Name = "Plastik display (Unit Test)"
                }, context);

                var product = ProductService.NewProduct(new Product
                {
                    Name = "Kør sportsvogn (Unit Test)",
                    Price = 199
                }, context);

                var stock = StockService.NewStock(store, displayType, null, 25, context);

                StockService.AddProductToStock(stock.Id, product.Id, 3, context);

                SalesService.RegisterSale(new Sale
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
                var store = StoreService.AddStore(new Store
                {
                    Name = "Bilka Skagen (Unit Test)"
                }, context);

                var displayType = DisplayTypeService.AddDisplayType(new DisplayType
                {
                    Capacity = 10,
                    Name = "Plastik display (Unit Test)"
                }, context);

                var product = ProductService.NewProduct(new Product
                {
                    Name = "Kør sportsvogn (Unit Test)",
                    Price = 199
                }, context);

                var stock = StockService.NewStock(store, displayType, null, 25, context);

                StockService.AddProductToStock(stock.Id, product.Id, 3, context);

                SalesService.RegisterSale(new Sale
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

        [Test]
        public void can_calculate_product_sales_percentage_correctly()
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
                    Name = "Plastik display (Unit Test)"
                }, context);

                var product1 = ProductService.NewProduct(new Product
                {
                    Name = "Kør sportsvogn (Unit Test)",
                    Price = 100
                }, context);

                var product2 = ProductService.NewProduct(new Product
                {
                    Name = "Spring faldskærm (Unit Test)",
                    Price = 100
                }, context);

                var stock = StockService.NewStock(store, displayType, null, 25, context);

                StockService.AddProductToStock(stock.Id, product1.Id, 10, context);
                StockService.AddProductToStock(stock.Id, product2.Id, 10, context);

                for (var i = 0; i < 3; i++)
                {
                    SalesService.RegisterSale(new Sale
                    {
                        Store = store,
                        Product = product1,
                        SalesPrice = 199,
                        DisplayType = displayType,
                        SalesDate = DateTime.Now,
                        IsReturn = false
                    }, context);
                }

                SalesService.RegisterSale(new Sale
                    {
                        Store = store,
                        Product = product2,
                        SalesPrice = 199,
                        DisplayType = displayType,
                        SalesDate = DateTime.Now,
                        IsReturn = false
                    }, context);

                var salesFractions = SalesService.GetProductsSalesFraction(store.Id, DateTime.Now.AddMinutes(-1), DateTime.Now, context);

                Assert.AreEqual(product1.Id, salesFractions[0].Item1.Id);
                Assert.AreEqual(0.75d, salesFractions[0].Item2);

                Assert.AreEqual(product2.Id, salesFractions[1].Item1.Id);
                Assert.AreEqual(0.25d, salesFractions[1].Item2);
            }
        }

        [Test]
        public void can_calculate_product_sales_percentage_correctly_including_returns()
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
                    Name = "Plastik display (Unit Test)"
                }, context);

                var product1 = ProductService.NewProduct(new Product
                {
                    Name = "Kør sportsvogn (Unit Test)",
                    Price = 100
                }, context);

                var product2 = ProductService.NewProduct(new Product
                {
                    Name = "Spring faldskærm (Unit Test)",
                    Price = 100
                }, context);

                var stock = StockService.NewStock(store, displayType, null, 25, context);

                StockService.AddProductToStock(stock.Id, product1.Id, 10, context);
                StockService.AddProductToStock(stock.Id, product2.Id, 10, context);

                for (var i = 0; i < 4; i++)
                {
                    SalesService.RegisterSale(new Sale
                    {
                        Store = store,
                        Product = product1,
                        SalesPrice = 199,
                        DisplayType = displayType,
                        SalesDate = DateTime.Now,
                        IsReturn = false
                    }, context);
                }

                SalesService.RegisterSale(new Sale //single return
                {
                    Store = store,
                    Product = product1,
                    SalesPrice = 199,
                    DisplayType = displayType,
                    SalesDate = DateTime.Now,
                    IsReturn = true
                }, context);

                SalesService.RegisterSale(new Sale
                {
                    Store = store,
                    Product = product2,
                    SalesPrice = 199,
                    DisplayType = displayType,
                    SalesDate = DateTime.Now,
                    IsReturn = false
                }, context);

                var salesFractions = SalesService.GetProductsSalesFraction(store.Id, DateTime.Now.AddMinutes(-1), DateTime.Now, context);

                Assert.AreEqual(product1.Id, salesFractions[0].Item1.Id);
                Assert.AreEqual(0.75d, salesFractions[0].Item2);

                Assert.AreEqual(product2.Id, salesFractions[1].Item1.Id);
                Assert.AreEqual(0.25d, salesFractions[1].Item2);
            }
        }

        [Test]
        public void can_get_sales()
        {
            using (var context = new StoreStockingContext())
            {
                // Get store sales from January 2015
                var fromDate = new DateTime(2015, 1, 1);
                var toDate = new DateTime(2015, 1, 31);
                var storeSales = SalesService.GetSales(99, fromDate, toDate, context);

                //System.Diagnostics.Debug.WriteLine("January sales: " + storeSales.Count);

                Assert.IsNotEmpty(storeSales);

       
                // Get chain sales from January 2015
                var chainSales = SalesService.GetChainSales(1, fromDate, toDate, context);

                //System.Diagnostics.Debug.WriteLine("January sales: " + chainSales.Count);

                Assert.IsNotEmpty(chainSales);

                // Check that the yearly sales correspond to monthly sales
                var yearSales = SalesService.GetYearSales(2015, 1, context);

                Assert.AreEqual(yearSales.First().Count, chainSales.Count);

                //System.Diagnostics.Debug.WriteLine("January sales: " + yearSales.First().Count);
            }
        }

    }
}