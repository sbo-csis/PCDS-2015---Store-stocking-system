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
            SalesPerson sp = new SalesPerson() {Name = "Person1"};
            DisplayType dt = new DisplayType() {Name = "display1" };
            Store store = new Store() { Name = "Store1", SalesPerson = sp};
            Stock stock = new Stock() { Store = store, Capacity = 20, DisplayType = dt };
            for (int i = 0; i < 100; i++)
            {
                Product p = new Product() { Name = String.Format("{0}", "Product" + i), Price = i};
                ProductStock productStock = new ProductStock() {Stock = stock, Amount = 10, WarningAmount = 10, Product = p};
                stock.ProductStock.Add(productStock);
            }
            ssc.Stocks.Add(stock);
            ssc.SaveChanges();
        }
    }
}