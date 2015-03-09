using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class SalesService
    {
        public static void RegisterSale(Sale newSale)
        {
            using (var context = new StoreStockingContext())
            {
                context.Sales.Add(newSale);
                context.SaveChanges();
            }

            // TODO: Should we update stock in the salesservice or make a job that does it with x minute intervals?
        }

        public static void RegisterSale(int storeId, int productId, int salesPrice, int displayTypeId, DateTime salesDate)
        {
            RegisterSale(new Sale
            {
                StoreId = storeId,
                ProductId = productId,
                SalesPrice = salesPrice,
                DisplayTypeId = displayTypeId,
                SalesDate = salesDate
            });
        }
        
        public static List<Sale> GetSales(int storeId, DateTime fromDate, DateTime toDate)
        {
            using (var context = new StoreStockingContext())
            {
                var sales = (from  t in context.Sales
                             where  t.StoreId == storeId
                             &&    (t.SalesDate > fromDate && t.SalesDate < toDate)
                             select t).ToList();

                return sales;
            }
        }

        public static void RemoveSale(Sale newSale)
        {
            using (var context = new StoreStockingContext())
            {
                context.Sales.Remove(newSale);
                context.SaveChanges();
            }
        }

        public static void RemoveSale(int storeId, int productId, int salesPrice, int displayTypeId, DateTime salesDate)
        {
            RemoveSale(new Sale
            {
                StoreId = storeId,
                ProductId = productId,
                SalesPrice = salesPrice,
                DisplayTypeId = displayTypeId,
                SalesDate = salesDate
            });
        }
    }
}