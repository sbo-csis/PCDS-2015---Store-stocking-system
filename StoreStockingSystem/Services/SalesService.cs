using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Web.DynamicData;
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

            return (from t in context.Sales
                    where t.Store.Id == storeId
                    && (t.SalesDate > fromDate && t.SalesDate < toDate)
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
                chainSales.AddRange(GetSales(store.Id, fromDate, toDate, context));
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
                var totalMonthSales = monthSales.Aggregate(0, (current, sale) => (int)(current + sale.SalesPrice));

                yearSales.Add(totalMonthSales);
            }

            return yearSales;
        }

        //Returns a list where each value is the predicted sale in the i'th month
        public static List<double> GetPredictedStoreSales(int storeId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            List<double> growthRateResults = new List<double>();

            //Make sure this is the oldest and not the newest
            int oldestSaleYear = (from sale in context.Sales
                                  where sale.StoreId == storeId
                                  orderby sale.SalesDate
                                  select sale.SalesDate).First().Year;
            int newestSaleYear = DateTime.UtcNow.Year - 1;

            for (int i = 1; i <= 12; i++)
            {
                DateTime newFromDate = new DateTime(newestSaleYear, i, 1);
                DateTime newToDate = newFromDate.AddMonths(1).AddDays(-1);
                List<Sale> newMonthSales = GetSales(storeId, newFromDate, newToDate, context);
                int totalNewMonthSales = newMonthSales.Aggregate(0, (current, sale) => (int)(current + sale.SalesPrice));

                DateTime oldFromDate = new DateTime(oldestSaleYear, i, 1);
                DateTime oldToDate = oldFromDate.AddMonths(1).AddDays(-1);
                List<Sale> oldMonthSales = GetSales(storeId, oldFromDate, oldToDate, context);
                int totalOldMonthSales = oldMonthSales.Aggregate(0, (current, sale) => (int)(current + sale.SalesPrice));

                growthRateResults.Add(Math.Pow(totalNewMonthSales / (double)totalOldMonthSales, 1.0 / (newestSaleYear - oldestSaleYear)) * totalNewMonthSales);
            }

            return growthRateResults;
        }

        public static List<Tuple<Product, decimal>> GetProductsSalesFraction(int storeId, DateTime startDate, DateTime endDate, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var productFractions = new List<Tuple<Product, decimal>>();

            var salesForPeriod = GetSales(storeId, startDate, endDate, context); // all sales from last 4 weeks for store

            var productIds = salesForPeriod.Select(t => t.ProductId).Distinct().ToList(); // list of distinct product IDs from sales

            foreach (var productId in productIds)
            {
                var productFraction = CalculateSingleProductSalesFraction(productId, salesForPeriod);

                productFractions.Add(productFraction);
            }

            return productFractions;
        }

        public static Tuple<Product, decimal> GetProductSalesFraction(int storeId, int productId, DateTime startDate, DateTime endDate, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var salesForPeriod = GetSales(storeId, startDate, endDate, context); // all sales from last 4 weeks for store

            var productFraction = CalculateSingleProductSalesFraction(productId, salesForPeriod);

            return productFraction;
        }

        // totalSalesSum is optional for increased performance, as we avoid having to sum over all sales repeatedly.
        private static Tuple<Product, decimal> CalculateSingleProductSalesFraction(int productId, List<Sale> totalSales, decimal totalSalesSum = 0)
        {
            if (totalSalesSum == 0)
            {
                var salesSum = (from t in totalSales
                                where !t.IsReturn
                                select t.SalesPrice).Sum();

                var returnSum = (from t in totalSales
                                 where t.IsReturn
                                 select t.SalesPrice).Sum();

                totalSalesSum = salesSum - returnSum;
            }

            var productSalesSum = (from t in totalSales
                                   // sum of all sales with product id parameter
                                   where t.ProductId == productId
                                         && !t.IsReturn
                                   select t.SalesPrice).Sum();

            var returnsSalesSum = (from t in totalSales
                                   where t.ProductId == productId
                                      && t.IsReturn
                                   select t.SalesPrice).Sum();

            var product = ProductService.GetProduct(productId);

            var fraction = (productSalesSum - returnsSalesSum) / totalSalesSum;

            return new Tuple<Product, decimal>(product, fraction);
        }
    }
}
