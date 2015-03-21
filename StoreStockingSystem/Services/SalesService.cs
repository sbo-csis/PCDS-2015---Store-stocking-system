using System;
using System.Collections.Generic;
using System.Linq;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class SalesService
    {
        public delegate void SaleOccuredEventHandler(Sale sale, StoreStockingContext context);
        public static event SaleOccuredEventHandler SalesEvent;


        public static void RegisterSale(Sale newSale, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            context.Sales.Add(newSale);
            context.SaveChanges();
            
            SalesEvent(newSale, context); // Make sales event. StockService subscribes to this, which in turn updates the stock for the given store.
        }

        public static void RegisterSale(int storeId, int productId, int salesPrice, int displayTypeId, bool isreturn, DateTime salesDate, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            RegisterSale(new Sale
            {
                Store = StoreService.GetStore(storeId, context),
                Product = ProductService.GetProduct(productId, context),
                SalesPrice = salesPrice,
                DisplayType = DisplayTypeService.GetDisplayType(displayTypeId, context),
                IsReturn = isreturn,
                SalesDate = salesDate
            }, context);
        }
        
        public static List<Sale> GetSales(int storeId, DateTime fromDate, DateTime toDate, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

                return (from  t in context.Sales
                        where  t.Store.Id == storeId
                        &&    (t.SalesDate > fromDate && t.SalesDate < toDate)
                        select t).ToList();
        }
    }
}