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

        public static void NewStock(Stock newStock)
        {
            using (var context = new StoreStockingContext())
            {
                context.Stocks.Add(newStock);
                context.SaveChanges();
            }
        }

        public static void NewStock(int storeId, int displayTypeId, int? capacity, int? warningPercentage)
        {
            var store = StoreService.GetStore(storeId);
            var displayType = DisplayTypeService.GetDisplayType(displayTypeId);

            NewStock(new Stock
            {
                Capacity = capacity ?? displayType.Capacity, // Default capacity equal to displaytypes standard capacity
                DisplayType = displayType,
                Store = store,
                WarningPercentageStockLeft = warningPercentage ?? 10 // Default warning at 10%
            });
        }

        public static Stock GetStock(Store store, int displayTypeId)
        {
            using (var context = new StoreStockingContext())
            {
                var stock = (from t in context.Stocks
                             where t.Store.Id == store.Id
                                && t.DisplayType.Id == displayTypeId
                             select t).FirstOrDefault();

                if (stock == null)
                    throw new ArgumentException("Could not find stock for store id: " + store.Id + " and display-type id" + displayTypeId);
                
                return stock;
            }
        }

        public static Stock GetStock(int storeId, int productId)
        {
            using (var context = new StoreStockingContext())
            {
                var store = context.Stores.Find(storeId);
                return GetStock(store, productId);
            }
        }

        public static List<Stock> GetStocks()
        {
            using (var context = new StoreStockingContext())
            {
                List<Stock> stocks = (from t in context.Stocks
                             select t).ToList();
                return stocks;
            }
        }

        public static void AddStock(Stock stock, int productId, int amount)
        {
            using (var context = new StoreStockingContext())
            {
                var productStock = (from t in stock.ProductStock
                                    where t.ProductId == productId
                                    select t).FirstOrDefault();

                if (productStock == null)
                    throw new ArgumentException("Could not find productid: " + stock.Id + " in stockid " + productId);

                productStock.Amount += amount;

                context.SaveChanges();
            }
        }

        public static void ModifyStockBasedOnSale(Sale sale)
        {
            var stock = GetStock(sale.StoreId, sale.DisplayTypeId);
            var amount = (sale.IsReturn ? -1 : 1);
            AddStock(stock, sale.ProductId, amount);
        }

    }
}