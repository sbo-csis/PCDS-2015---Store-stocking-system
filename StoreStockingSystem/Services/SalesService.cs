using System;
using System.Collections;
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

        public static List<Sale> GetChainSales(int chainId, DateTime fromDate, DateTime toDate, StoreStockingContext context)
        {
            if (context == null)
                context = new StoreStockingContext();

            // Get stores in chain
            var chainStores = ChainService.GetChainStores(chainId, context);

            // Get accumulated sales for the chain stores 
            List<Sale> chainSales = new List<Sale>();
            foreach (Store store in chainStores)
            {
                chainSales.AddRange(SalesService.GetSales(store.Id, fromDate, toDate, context));
            }

            return chainSales;
        }

        public static ArrayList GetYearSales(int year, int chainId, StoreStockingContext context)
        {
            if (context == null)
                context = new StoreStockingContext();

            var yearSales = new ArrayList(12);

            for (int i = 1; i <= 12; i++)
            {
                var firstDayOfMonth = new DateTime(year, i, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var monthSales = GetChainSales(chainId, firstDayOfMonth, lastDayOfMonth, context);

                // Accumulate sale values to one data point
                var totalMonthSales = monthSales.Aggregate(0, (current, sale) => current + sale.SalesPrice);

                yearSales.Add(totalMonthSales);
            }

            return yearSales;
        }
    }
}