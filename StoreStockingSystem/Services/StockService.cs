using System;
using System.Collections.Generic;
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
                WarningPercentageStockLeft = warningPercentage ?? 10 // Default warning at 10%
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
                WarningPercentageStockLeft = warningPercentage ?? 10 // Default warning at 10%
            }, context);
        }

        public static Stock GetStock(int stockId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return (from t in context.Stocks
                    where t.Id == stockId
                    select t).FirstOrDefault();
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

        public static List<Stock> GetAllStocks(StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();
            
            return (from t in context.Stocks
                          select t).ToList();
        }

        public static void AddProductToStock(Stock stock, Product product, int amount, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var productStock = (stock.ProductStock != null) ? (from t in stock.ProductStock
                                                                where t.ProductId == product.Id
                                                                select t).FirstOrDefault()
                                                            : null;

            if (productStock == null) //Adding new product to stock
            {
                context.ProductStocks.Add((new ProductStock
                {
                    Amount = amount,
                    ProductId = product.Id,
                    StockId = stock.Id
                }));
            }
            else //Updating already existing product to stock
            {
                productStock.Amount += amount;
            }

            context.SaveChanges();
        }

        public static void AddProductToStock(int stockId, int productId, int amount, StoreStockingContext context)
        {
            if(context == null)
                context = new StoreStockingContext(); 
    
            var stock = context.Stocks.Find(stockId);
            if (stock == null)
                throw new ArgumentException("Failed adding product to stock. Could not find stock id: " + stockId);

            var product = context.Products.Find(productId);
            if (product == null)
                throw new ArgumentException("Failed adding product to stock. Could not find product id: " + productId);

            AddProductToStock(stock, product, amount, context);
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

            return (from t in stock.ProductStock
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
    }
}