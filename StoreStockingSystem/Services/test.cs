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
            Store store1 = new Store() { Name = "Store1", SalesPerson = sp};
            Stock stock1 = new Stock() { Store = store1, Capacity = 20, DisplayType = dt };
            for (int i = 0; i < 20; i++)
            {
                Product p = new Product() { Name = String.Format("{0}", "Product" + i), Price = i};
                ProductStock productStock1 = new ProductStock() {Stock = stock1, Amount = 10, WarningAmount = 10, Product = p};
                ProductStock productStock2 = new ProductStock() { Stock = stock1, Amount = 15, WarningAmount = 10, Product = p };
                ProductStock productStock3 = new ProductStock() { Stock = stock1, Amount = 20, WarningAmount = 10, Product = p };
                stock1.ProductStock.Add(productStock1);
                stock1.ProductStock.Add(productStock2);
                stock1.ProductStock.Add(productStock3);
            }
            
            ssc.Stocks.Add(stock1);
            ssc.SaveChanges();
        }
    }
}