using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class StockService
    {
        static StockService()
        {
            SalesService.SalesEvent += ModifyStockBasedOnSale;
        }

        public static Stock NewStock(Stock newStock, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            if (newStock.Store != null)
                context.Stores.Attach(newStock.Store);

            if (newStock.DisplayType != null)
                context.DisplayTypes.Attach(newStock.DisplayType);

            context.Stocks.Add(newStock);
            context.SaveChanges();

            return newStock;
        }

        public static Stock NewStock(Store store, DisplayType displayType, int? capacity, int? warningPercentage, StoreStockingContext context = null)
        {
            return NewStock(new Stock
            {
                Capacity = capacity ?? displayType.Capacity, // Default capacity equal to displaytypes standard capacity
                DisplayTypeId = displayType.Id,
                StoreId = store.Id,
                WarningAmountLeft = warningPercentage ?? 10 // Default warning at 10%
            }, context);
        }

        public static Stock NewStock(int storeId, int displayTypeId, int? capacity, int? warningPercentage, StoreStockingContext context = null)
        {
            var store = StoreService.GetStore(storeId, context);
            var displayType = DisplayTypeService.GetDisplayType(displayTypeId, context);

            return NewStock(new Stock
            {
                Capacity = capacity ?? displayType.Capacity, // Default capacity equal to displaytypes standard capacity
                DisplayType = displayType,
                Store = store,
                WarningAmountLeft = warningPercentage ?? 10 // Default warning at 10%
            }, context);
        }

        public static Stock GetStockWithEmptyDate(Stock stock, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            if (stock == null)
                return null;

            foreach (var productStock in stock.ProductStocks)
            {
                var warningDate = PredictProductTargetStockLevelDate(productStock, productStock.WarningAmount ?? stock.WarningAmountLeft, context);
                productStock.ExpectedWarningDate = warningDate;
            }

            return stock;
        }


        public static Stock GetStock(int stockId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var stock = (from t in context.Stocks
                         where t.Id == stockId
                         select t).FirstOrDefault();

            return stock;
        }

        public static Stock GetStock(Store store, int displayTypeId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var stock = (from t in context.Stocks
                         where t.Store.Id == store.Id
                         && t.DisplayType.Id == displayTypeId
                         select t).FirstOrDefault();

            if (stock == null)
                throw new ArgumentException("Could not find stock for store id: " + store.Id + " and display-type id " + displayTypeId);

            return stock;
        }

        public static List<Stock> GetStocks(Store store, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var stocks = (from t in context.Stocks
                          where t.Store.Id == store.Id
                          select t)
                         .Include(t => t.DisplayType)
                         .Include(t => t.Store)
                         .Include(t => t.ProductStocks.Select(s => s.Product))
                         .ToList();

            if (stocks == null)
                throw new ArgumentException("Could not find stocks for store id: " + store.Id);

            return stocks;
        }

        public static ProductStock GetProductStock(Stock stock, Product product, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return (from t in context.ProductStocks
                    where t.Stock.Id == stock.Id && t.Product.Id == product.Id
                    select t).FirstOrDefault();
        }

        public static ProductStock GetProductStock(int stockId, int productId, StoreStockingContext context = null)
        {
            return GetProductStock(GetStock(stockId), ProductService.GetProduct(productId), context);
        }

        public static List<ProductStock> GetAllProductStocks(int stockId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return (from t in context.ProductStocks
                    where t.Stock.Id == stockId
                    select t).ToList();
        }

        public static IEnumerable<Stock> GetAllStocks(StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return (from t in context.Stocks
                    select t);
        }

        public static void AddProductToStock(Stock stock, Product product, int amount, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var productStock = (stock.ProductStocks != null) ? (from t in stock.ProductStocks
                                                                where t.ProductId == product.Id
                                                                select t).FirstOrDefault()
                                                            : null;

            if (productStock == null) //Adding new product to stock
            {
                context.ProductStocks.Add((new ProductStock
                {
                    CurrentAmount = amount,
                    ProductId = product.Id,
                    StockId = stock.Id
                }));
            }
            else //Updating already existing product to stock
            {
                productStock.CurrentAmount += amount;
            }

            context.SaveChanges();
        }

        public static void AddProductToStock(int stockId, int productId, int amount, StoreStockingContext context)
        {
            if (context == null)
                context = new StoreStockingContext();

            var stock = context.Stocks.Find(stockId);
            if (stock == null)
                throw new ArgumentException("Failed adding product to stock. Could not find stock id: " + stockId);

            var product = context.Products.Find(productId);
            if (product == null)
                throw new ArgumentException("Failed adding product to stock. Could not find product id: " + productId);

            AddProductToStock(stock, product, amount, context);
            StoreService.SetRefillFlag(false, stock.StoreId, context); //When adding stock manually, this counts as a refill.
        }

        public static void ModifyStockBasedOnSale(Sale sale, StoreStockingContext context = null)
        {
            var productStock = GetProductStockBasedOnSalesData(sale.DisplayType, sale.Store, sale.Product, context);
            var amount = (sale.IsReturn ? 1 : -1); // add 1 back to stock if return, otherwise remove 1 from stock.
            AddProductToStock(productStock.StockId, sale.Product.Id, amount, context);
        }

        private static ProductStock GetProductStockBasedOnSalesData(DisplayType displayType, Store store, Product product, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var stock = (from t in context.Stocks
                         where t.Store.Id == store.Id && t.DisplayType.Id == displayType.Id
                         select t).FirstOrDefault();

            if (stock == null)
                throw new ArgumentException("No stock for store \"" + store.Name + "\" with a display type: \"" + displayType.Name + "\"");

            return (from t in stock.ProductStocks
                    where t.ProductId == product.Id
                    select t).FirstOrDefault();
        }

        public static void RemoveProductFromStock(Stock stock, Product product, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var productStock = (from t in context.ProductStocks
                                where t.Stock.Id == stock.Id && t.Product.Id == product.Id
                                select t).FirstOrDefault();

            if (productStock == null)
                throw new ArgumentException("Could not find any product: \"" + product + "\" in stock id " + stock.Id);

            context.ProductStocks.Remove(productStock);
            context.SaveChanges();
        }

        public static void RemoveProductFromStock(int stockId, int productId, StoreStockingContext context = null)
        {
            RemoveProductFromStock(GetStock(stockId, context), ProductService.GetProduct(productId, context), context);
        }

        public static List<Stock> GetStocksNeedingRefilling(StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var stocks = (from t in context.Stocks
                          select t)
                          .Include(t => t.ProductStocks)
                          .Include(t => t.ProductStocks.Select(p => p.Product))
                          .Include(t => t.DisplayType)
                          .Include(t => t.Store)
                          .ToList();

            var result = new List<Stock>();

            foreach (var stock in stocks)
            {
                foreach (var productStock in stock.ProductStocks)
                {
                    if(productStock.CurrentAmount < productStock.Capacity)
                        result.Add(stock);
                    break;
                }
            }

            return result;
        }

        // Returns a StockRefill object
        public static StockRefill GetLowStocks(StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var result = new List<Stock>();

            var stocks =
                context.Stocks
                .Include(t => t.ProductStocks)
                .Include(t => t.DisplayType)
                .Include(t => t.ProductStocks.Select(s => s.Product))
                .Include(t => t.Store)
                    .ToList();

            foreach (var stock in stocks)
            {
                var productStocks = (from t in stock.ProductStocks
                                     where t.CurrentAmount <= t.WarningAmount || t.CurrentAmount <= stock.WarningAmountLeft
                                     select t).ToList();

                if (productStocks.Count > 0)
                    result.Add(stock);
            }

            var stockRefill = new StockRefill
            {
                Stocks = result,
                RefillResponseible = null
            };

            //return NewStock(new Stock
            //{
            //    Capacity = capacity ?? displayType.Capacity, // Default capacity equal to displaytypes standard capacity
            //    DisplayTypeId = displayType.Id,
            //    StoreId = store.Id,
            //    WarningAmountLeft = warningPercentage ?? 10 // Default warning at 10%
            //}, context);

            return stockRefill;
        }

        /// <summary>
        /// Returns a list of productstocks and the dates of when they hit their corresponding warning levels.
        /// </summary>
        /// <param name="stockId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Tuple<ProductStock, DateTime>> PredictStockWarningLevelDates(int stockId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var stock = (from t in context.Stocks
                         where t.Id == stockId
                         select t).FirstOrDefault();

            if (stock == null)
                throw new Exception("Could not find stock");

            return GetPredictedStockTargetDate(stockId, stock.WarningAmountLeft, context);
        }

        /// <summary>
        /// Returns a list of productstocks and the dates of when they hit their corresponding empty levels.
        /// </summary>
        /// <param name="stockId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static List<Tuple<ProductStock, DateTime>> GetPredictedStockEmptyLevelDates(int stockId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var predictedDate = GetPredictedStockTargetDate(stockId, 0, context);

            return predictedDate;
        }

        /// <summary>
        /// Get a list of each product that needs refilling and the count for each product.
        /// </summary>
        /// <param name="storeId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public static IEnumerable<Tuple<ProductStock, int>> GetProductRefillList(int storeId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var productStocks = (from t in context.ProductStocks
                                 where t.Stock.StoreId == storeId
                                 select t)
                                 .Include(t => t.Stock)
                                 .Include(t => t.Stock.Store)
                                 .Include(t => t.Stock.DisplayType)
                                 .Include(t => t.Product);

            var result = new List<Tuple<ProductStock, int>>();

            foreach (var productStock in productStocks)
            {
                result.Add(new Tuple<ProductStock, int>(productStock, productStock.Capacity - productStock.CurrentAmount));
            }

            return result;
        }

        /// <summary>
        /// Internal method to calculate when a stock hits a specific product count.
        /// </summary>
        /// <param name="stockId"></param>
        /// <param name="targetStockLevel"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static List<Tuple<ProductStock, DateTime>> GetPredictedStockTargetDate(int stockId, int targetStockLevel, StoreStockingContext context)
        {
            var stock = (from t in context.Stocks
                         where t.Id == stockId
                         select t).Include(t => t.ProductStocks).FirstOrDefault(); //Fetch the product stocks now to avoid lazy loading later.

            if (stock == null)
                throw new Exception("Stock not found for stock id " + stockId);

            if (targetStockLevel < 0) // Defensive programming to avoid illogical even of having negative stock.
                targetStockLevel = 0;

            var result = new List<Tuple<ProductStock, DateTime>>();

            foreach (var productStock in stock.ProductStocks)
            {
                var psTargetDate = PredictProductTargetStockLevelDate(productStock, targetStockLevel, context);

                result.Add(new Tuple<ProductStock, DateTime>(productStock, psTargetDate));
            }

            return result;
        }

        /// <summary>
        /// Gives the date when a stock hits the wanted level, based on sales speed of a specific productstock (i.e. a product in a specific store)
        /// </summary>
        /// <param name="productStock"></param>
        /// <param name="targetStockLevel"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private static DateTime PredictProductTargetStockLevelDate(ProductStock productStock, int targetStockLevel, StoreStockingContext context)
        {
            //TODO: Refactor the dates out, either to constants or to parameters in the method signature.
            //Currently 30 days future period hardcoded as the period we want sales speed for.
            var saleSpeed = SalesService.BuildSaleSpeedForProductStock(productStock, DateTime.Now, DateTime.Now.AddDays(30));

            var currentStockCount = productStock.CurrentAmount;

            var daysUntilTargetLevel = 0;

            while (targetStockLevel < currentStockCount)
            {
                currentStockCount = (int)(currentStockCount - Math.Ceiling(saleSpeed.SalesCountPerDay));
                daysUntilTargetLevel++;
            }

            return DateTime.Now.AddDays(daysUntilTargetLevel);
        }
    }
}