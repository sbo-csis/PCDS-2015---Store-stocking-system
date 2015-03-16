using System;
using System.Collections.Generic;
using System.Linq;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class SalesService
    {
        public delegate void SaleOccuredEventHandler(Sale sale);
        public static event SaleOccuredEventHandler SalesEvent;


        public static void RegisterSale(Sale newSale)
        {
            using (var context = new StoreStockingContext())
            {
                context.Sales.Add(newSale);
                context.SaveChanges();
                SalesEvent(newSale); // Make sales event. StockService subscribes to this, which in turn updates the stock for the given store.
            }
        }

        public static void RegisterSale(int storeId, int productId, int salesPrice, int displayTypeId, bool isreturn, DateTime salesDate)
        {
            RegisterSale(new Sale
            {
                StoreId = storeId,
                ProductId = productId,
                SalesPrice = salesPrice,
                DisplayTypeId = displayTypeId,
                IsReturn = isreturn,
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
    }
}