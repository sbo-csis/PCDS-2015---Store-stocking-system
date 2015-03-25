using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class Test
    {
        public static void Seed(StoreStockingContext ssc)
        {
            SalesPerson sp = new SalesPerson() {Id = 1, Name = "Person1"};
            DisplayType dt = new DisplayType() { Id = 1, Name = "display1" };
            Store store = new Store() { Name = "Store1", Id = 1, SalesPerson = sp, SalesPersonId = 1};
            Stock stock = new Stock() { Id = 1, Store = store, StoreId = 1, WarningMaountLeft = 20, Capacity = 20, DisplayType = dt, DisplayTypeId = 1 };
            List<ProductStock> psList = new List<ProductStock>();
            for (int i = 0; i < 100; i++)
            {
                Product p = new Product() { Name = String.Format("{0}", i), Price = i, Id = i };
                ProductStock productStock = new ProductStock() { Id = 1, Stock = stock, StockId = 1, ProductId = i, Amount = 10, WarningAmount = 10 };
                psList.Add(productStock);
                ssc.Products.Add(p);
                ssc.ProductStocks.Add(productStock);
            }
            stock.ProductStock = psList;
            ssc.Stores.Add(store);
            ssc.Stocks.Add(stock);
            ssc.SaveChanges();
        }
    }
}