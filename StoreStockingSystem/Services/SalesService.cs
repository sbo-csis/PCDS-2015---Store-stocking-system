using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class SalesService
    {
        public class SaleSpeed
        {
            public ProductStock ProductStock { get; set; }
            public double SalesCountPerDay { get; set; }
            public decimal SalesSumPerDay { get; set; }
        }

        public class SalesWarningInfo
        {
            public SaleSpeed PredictedSaleSpeed { get; set; }
            public SaleSpeed ActualSaleSpeed { get; set; }
            public Product Product { get; set; }
        }

        public class RefillWarningInfo
        {
            public DateTime ExpectedDateOutOfStock { get; set; }
            public int AmountLeft { get; set; }
            public Product Product { get; set; }
        }

        public delegate void SaleOccuredEventHandler(Sale sale, StoreStockingContext context);
        public static event SaleOccuredEventHandler SalesEvent;

        // TODO: This does not update stock currently because of performance issues!
        public static void RegisterSales(List<Sale> sales, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            //if (SalesEvent != null) //Comment out to gain performance.
            //{
            //    foreach (var sale in sales)
            //    {
            //        SalesEvent(sale, context);
            //    }
            //}
            
            context.Sales.AddRange(sales);
            context.SaveChanges();
        }

        public static void RegisterSale(Sale sale, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            context.Sales.Add(sale);
            context.SaveChanges();

            if(SalesEvent != null)
                SalesEvent(sale, context); // Make sales event. StockService subscribes to this, which in turn updates the stock for the given store.
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

        // Returns a list of sales for the given store in the given period
        public static List<Sale> GetSales(int storeId, DateTime fromDate, DateTime toDate, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return (from t in context.Sales
                    where t.Store.Id == storeId
                    && (t.SalesDate > fromDate && t.SalesDate < toDate)
                    select t).ToList();
        }

        public static List<Sale> GetSalesBasedOnProductStock(ProductStock productStock, DateTime fromDate, DateTime toDate, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return (from t in context.Sales
                    where t.Store.Id == productStock.Stock.StoreId
                       && t.ProductId == productStock.ProductId
                       && t.DisplayTypeId == productStock.Stock.DisplayTypeId
                    && (t.SalesDate > fromDate && t.SalesDate < toDate)
                    select t).ToList();
        }

        // Returns a list of sales for the given chain in the given period
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

        // Get a list of 12 lists, each corresponding to a months sales
        public static List<List<Sale>> GetYearSales(int year, int chainId, StoreStockingContext context)
        {
            if (context == null)
                context = new StoreStockingContext();

            var yearSales = new List<List<Sale>>(12);

            for (int i = 1; i <= 12; i++)
            {
                var firstDayOfMonth = new DateTime(year, i, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

                var monthSales = GetChainSales(chainId, firstDayOfMonth, lastDayOfMonth, context);

                // Accumulate sale values to one data point
                //var totalMonthSales = monthSales.Aggregate(0, (current, sale) => (int)(current + sale.SalesPrice));

                yearSales.Add(monthSales);
            }

            return yearSales;
        }


        //Returns a list where each value is the predicted sale in the i'th month
        //TODO: change logic & refactor so it is possible to get expected sales sum for a specific start and end date
        //TODO: change logic & refactor so it is possible to get expected sales sum all the stores products for a specific start and end date
        public static List<double> GetPredictedStoreSales(int storeId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            List<double> growthRateResults = new List<double>();

            int oldestSaleYear = (from sale in context.Sales
                                  where sale.StoreId == storeId
                                  orderby sale.SalesDate ascending
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

                if (oldestSaleYear >= newestSaleYear)
                {
                    growthRateResults.Add(totalOldMonthSales);
                    continue;
                }

                growthRateResults.Add(Math.Pow(totalNewMonthSales / (double)totalOldMonthSales, 1.0 / (newestSaleYear - oldestSaleYear)) * totalNewMonthSales);
            }

            return growthRateResults;
        }


        //For each store, for each product, if actual sales are <Store.WarningPercentage> or less from predicted sales,
        //we return a class with the actual sales speed, predicted sales speed, store and product.
        //The result returned is sorted by Store.StorePriority aka ABC (here known as 1,2,3)
        public static List<KeyValuePair<Store, List<SalesWarningInfo>>> GetStoreSalesWarnings(StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            Dictionary<Store, List<SalesWarningInfo>> result = new Dictionary<Store, List<SalesWarningInfo>>();

            IEnumerable stores = from store in context.Stores
                orderby store.StorePriority ascending
                select store;

            foreach (Store store in stores)
            {
                int storeId = store.Id;
                DateTime from = DateTime.Today.AddDays(-8);
                DateTime to = DateTime.Today.AddDays(-1);
                List<SaleSpeed> predictedSalesSpeeds = GetPredictedStoreSalesForPeriod(storeId, from, to, context);
                foreach (SaleSpeed predictedSaleSpeed in predictedSalesSpeeds)
                {
                    Product product = predictedSaleSpeed.ProductStock.Product;
                    List<Sale> actualProductSales = context.Sales.Where(sale => sale.StoreId == storeId && sale.SalesDate <= to && sale.SalesDate >= from && sale.Product == product).ToList();
                    SaleSpeed actualProductSalesSpeed = CalculateSaleSpeedBasedOnCurrentSales(actualProductSales);
                    double comparedSalesCountPerDay = (100 / predictedSaleSpeed.SalesCountPerDay) * actualProductSalesSpeed.SalesCountPerDay;
                    decimal comparedSalesSumPerDay = (100 / predictedSaleSpeed.SalesSumPerDay) * actualProductSalesSpeed.SalesSumPerDay;
                    if ((comparedSalesCountPerDay < store.WarningPercentage) || (comparedSalesSumPerDay < store.WarningPercentage))
                    {
                        SalesWarningInfo salesWarningInfo = new SalesWarningInfo();
                        salesWarningInfo.ActualSaleSpeed = actualProductSalesSpeed;
                        salesWarningInfo.PredictedSaleSpeed = predictedSaleSpeed;
                        salesWarningInfo.Product = product;
                        if (result.ContainsKey(store))
                        {
                            result[store].Add(salesWarningInfo);
                            continue;
                        }

                        result[store] = new List<SalesWarningInfo>() { salesWarningInfo };
                    }
                }
            }

            var sortedResults = result.ToList();
                sortedResults.Sort((firstPair, nextPair) => firstPair.Key.StorePriority.CompareTo(nextPair.Key.StorePriority));

            return sortedResults;
        }

        //For each product for each store, if the predicted empty level date is less than 7 days from now, warn,
        //if the amount left is less than warning amount, warn.
        //The result returned is sorted by Store.StorePriority aka ABC (here known as 1,2,3)
        public static List<KeyValuePair<Store, List<RefillWarningInfo>>> GetStoreRefillWarnings(StoreStockingContext context = null)
        {
            if (context == null)
            {
                context = new StoreStockingContext();
            }

            DateTime minimum = DateTime.Today.AddDays(7);
            Dictionary<Store, List<RefillWarningInfo>> result = new Dictionary<Store, List<RefillWarningInfo>>();

            foreach (Stock stock in context.Stocks)
            {
                List<Tuple<ProductStock, DateTime>> outOfStockDate = StockService.GetPredictedStockEmptyLevelDates(stock.Id, context);
                foreach (Tuple<ProductStock, DateTime> productStock in outOfStockDate)
                {
                    if (productStock.Item2 < minimum || productStock.Item1.Amount < productStock.Item1.WarningAmount)
                    {
                        //TODO: It should sort by ABC first, date/amount second.
                        RefillWarningInfo refillWarninginfo =  new RefillWarningInfo();
                        refillWarninginfo.AmountLeft = productStock.Item1.Amount;
                        refillWarninginfo.ExpectedDateOutOfStock = productStock.Item2.Date;
                        refillWarninginfo.Product = productStock.Item1.Product;
                        if (result.ContainsKey(stock.Store))
                        {
                            result[stock.Store].Add(refillWarninginfo);
                            continue;
                        }
                        result[stock.Store] = new List<RefillWarningInfo> { refillWarninginfo };
                    }
                }
            }

            var sortedResults = result.ToList();
            sortedResults.Sort((firstPair, nextPair) => firstPair.Key.StorePriority.CompareTo(nextPair.Key.StorePriority));

            return sortedResults;
        }

        public static List<SaleSpeed> GetPredictedStoreSalesForPeriod(int storeId, DateTime startDate, DateTime endDate, StoreStockingContext context)
        {
            if (context == null)
                context = new StoreStockingContext();

            IEnumerable<Stock> stocks = (from t in context.Stocks // Get all stocks for the current store
                          where t.StoreId == storeId
                          select t).Include(t => t.ProductStocks);

            List<SaleSpeed> saleSpeeds = BuildSaleSpeedsForStocks(stocks, startDate, endDate);

            return saleSpeeds;
        }

        private static List<SaleSpeed> BuildSaleSpeedsForStocks(IEnumerable<Stock> stocks, DateTime startDate, DateTime endDate)
        {
            var result = new List<SaleSpeed>();

            foreach (var stock in stocks)
            {
                result.AddRange(BuildSaleSpeedsForSingleStock(stock, startDate, endDate));
            }

            return result;
        }

        private static IEnumerable<SaleSpeed> BuildSaleSpeedsForSingleStock(Stock stock, DateTime startDate, DateTime endDate)
        {
            var result = new List<SaleSpeed>();

            foreach (var productStock in stock.ProductStocks)
            {
                result.Add(BuildSaleSpeedForProductStock(productStock, startDate, endDate));
            }

            return result;
        }

        public static SaleSpeed BuildSaleSpeedForProductStock(ProductStock productStock, DateTime startDate, DateTime endDate)
        {
            using (var context = new StoreStockingContext())
            {
                var sales = GetSalesBasedOnProductStock(productStock, new DateTime(1900, 1, 1), DateTime.Now, context);

                var saleSpeed = CalculateSaleSpeed(sales, startDate, endDate, context);

                saleSpeed.ProductStock = productStock;

                return saleSpeed;
            }
        }

        /// <summary>
        /// Creates SaleSpeed object for a given number of sales. The SaleSpeed is calculated based on either historic sales yeears
        /// if they exist. Otherwise the average salespeed for all previous sales is used, e.g. all sales divided by days since first sale.
        /// </summary>
        /// <param name="sales"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static SaleSpeed CalculateSaleSpeed(List<Sale> sales, DateTime startDate, DateTime endDate, StoreStockingContext context)
        {
            var salesPerYear = CalculateSalesPerYear(sales, startDate, endDate);

            var salesIncreaseFactor = CalculateSalesIncreaseFactor(salesPerYear);

            if(salesIncreaseFactor == 1) //If not enough historic data, we use average sales speed for sales so far instead.
                return CalculateSaleSpeedBasedOnCurrentSales(sales);

            var latestYearSales = GetLatestYearSales(salesPerYear);

            var daysInSalePeriod = (endDate - startDate).TotalDays;

            var saleSpeed = new SaleSpeed
            {
                SalesCountPerDay = latestYearSales.Count/daysInSalePeriod,
                SalesSumPerDay = latestYearSales.Select(t => t.SalesPrice).Sum()/(decimal) daysInSalePeriod
            };

            return saleSpeed;
        }

        /// <summary>
        /// Returns all sales for the latest (highest year int) in supplied list of sales.
        /// </summary>
        /// <param name="salesPerYear"></param>
        /// <returns></returns>
        private static List<Sale> GetLatestYearSales(List<Tuple<DateTime, List<Sale>>> salesPerYear)
        {
            var latestYear = salesPerYear.Select(t => t.Item1.Year).Max();
            var sales = salesPerYear.FirstOrDefault(t => t.Item1.Year == latestYear);

            if(sales == null)
                throw new Exception("Logical error: Could not fetch sales in GetLatestYearSales method.");

            return sales.Item2;
        }

        /// <summary>
        /// Calculates SaleSpeed based on supplied sales. Simply takes average of sales sum and sales count for all supplied sales.
        /// </summary>
        /// <param name="sales"></param>
        /// <returns></returns>
        private static SaleSpeed CalculateSaleSpeedBasedOnCurrentSales(List<Sale> sales)
        {
            var salesSum = sales.Where(t => !t.IsReturn).Select(t => t.SalesPrice).Sum() - sales.Where(t => t.IsReturn).Select(t => t.SalesPrice).Sum();
            var salesCount = sales.Count(t => !t.IsReturn) - sales.Count(t => t.IsReturn);

            var daysOfSales = (sales.Select(t => t.SalesDate).Max() - sales.Select(t=> t.SalesDate).Min()).TotalDays; // Total number of days sales have happened in

            if (daysOfSales == 0.0 && salesCount > 0) //In the case of only a single sale in the period.
            {
                daysOfSales = (DateTime.Now - sales.First().SalesDate).TotalDays; 
            }

            return new SaleSpeed
            {
                SalesCountPerDay = (daysOfSales > 0 ) ? (salesCount/daysOfSales) : 0,
                SalesSumPerDay = (daysOfSales > 0) ? salesSum/(decimal) daysOfSales : 0
            };
        }

        /// <summary>
        ///         /// Calculates sales sum in the period between startdate and enddate. Returns a list of tuples where the first element is 
        /// the year and the second element is the sales sum for that period.
        /// </summary>
        private static List<Tuple<DateTime, List<Sale>>> CalculateSalesPerYear(List<Sale> sales, DateTime startDate, DateTime endDate)
        {
            var result = new List<Tuple<DateTime, List<Sale>>>();

            var salesYears = sales.Select(t => t.SalesDate.Year).Distinct().ToList();

            salesYears.Remove(DateTime.Now.Year); //remove current year, we only want historic years

            foreach (var year in salesYears)
            {
                var periodSales = (from t in sales
                                   where t.SalesDate > new DateTime(year, startDate.Month, startDate.Day)
                                      && t.SalesDate < new DateTime(year, endDate.Month, endDate.Day)
                                   select t).ToList();

                result.Add(new Tuple<DateTime, List<Sale>>(new DateTime(year), periodSales));
            }

            return result;
        }

        /// <summary>
        /// Calculates the average increase/decrease factor of previous years sales. Can be used to guess if the sales
        /// will rise or fall, and by how much, depending on historic data. If there is not enough historic data (minimum 2 previous years),
        /// then a factor of 1 is returned.
        /// </summary>
        /// <param name="salesPerYear"></param>
        /// <returns></returns>
        private static decimal CalculateSalesIncreaseFactor(List<Tuple<DateTime, List<Sale>>> salesPerYear)
        {
            if (salesPerYear.Count < 2) //If true, then there is no historic data, and we cannot infer a sales factor increase.
                return 1;

            var increasePerYear = new List<decimal>();

            var i = 0;
            while (i < salesPerYear.Count)
            {
                var saleSumNextYear = salesPerYear[i + 1].Item2.Where(t => !t.IsReturn).Select(t => t.SalesPrice).Sum()
                                          - salesPerYear[i + 1].Item2.Where(t => t.IsReturn).Select(t => t.SalesPrice).Sum();

                var saleSumPreviousYear = salesPerYear[i].Item2.Where(t => !t.IsReturn).Select(t => t.SalesPrice).Sum()
                                          - salesPerYear[i].Item2.Where(t => t.IsReturn).Select(t => t.SalesPrice).Sum();

                increasePerYear.Add(saleSumNextYear / saleSumPreviousYear);
                i++;
            }

            return increasePerYear.Average();
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


        // Get store performance, ie. total sale value, for a time period
        public static int GetStorePerformance(int storeId, DateTime fromDate, DateTime toDate, StoreStockingContext context)
        {
            if (context == null)
                context = new StoreStockingContext();

            var sales = GetSales(storeId, fromDate, toDate, context);

            var totalSales = sales.Aggregate(0, (current, sale) => (int)(current + sale.SalesPrice));

            return totalSales;
        }

        // Get daily performance in a period for a store
        public static List<int> GetDailyStorePerformance(int storeId, DateTime fromDate, DateTime toDate,
            StoreStockingContext context)
        {
            var dailyPerformance = new List<int>();

            for (var day = fromDate.Date; day.Date <= toDate.Date; day = day.AddDays(1))
            {
                var sales = GetStorePerformance(storeId, day, day.AddDays(1), context);

                dailyPerformance.Add(sales);
            }

            return dailyPerformance;
        }

        // Get monthly performance for a store
        public static List<int> GetMonthlyStorePerformance(int storeId, DateTime fromDate, DateTime toDate,
            StoreStockingContext context)
        {
            var monthlyPerformance = new List<int>();

            for (var day = fromDate.Date; day.Date <= toDate.Date; day = day.AddMonths(1))
            {
                var sales = GetStorePerformance(storeId, day, day.AddMonths(1), context);

                monthlyPerformance.Add(sales);
            }

            return monthlyPerformance;
        }

        // Get performance for a chain
        public static int GetChainPerformance(int chainId, DateTime fromDate, DateTime toDate, StoreStockingContext context)
        {
            if (context == null)
                context = new StoreStockingContext();

            var sales = GetChainSales(chainId, fromDate, toDate, context);

            var totalSales = sales.Aggregate(0, (current, sale) => (int)(current + sale.SalesPrice));

            return totalSales;
        }

        // Get daily performance in a period for a chain
        public static List<int> GetDailyChainPerformance(int chainId, DateTime fromDate, DateTime toDate,
            StoreStockingContext context)
        {
            var dailyPerformance = new List<int>();

            for (var day = fromDate.Date; day.Date <= toDate.Date; day = day.AddDays(1))
            {
                var sales = GetChainPerformance(chainId, day, day.AddDays(1), context);

                dailyPerformance.Add(sales);
            }

            return dailyPerformance;
        }

        // Get monthly performance for a chain
        public static List<int> GetMonthlyChainPerformance(int chainId, DateTime fromDate, DateTime toDate,
            StoreStockingContext context)
        {
            var monthlyPerformance = new List<int>();

            for (var day = fromDate.Date; day.Date <= toDate.Date; day = day.AddMonths(1))
            {
                var sales = GetChainPerformance(chainId, day, day.AddMonths(1), context);

                monthlyPerformance.Add(sales);
            }

            return monthlyPerformance;
        }
    }
}
