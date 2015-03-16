using System;
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

        public static Stock GetStock(int storeid)
        {
            using (var context = new StoreStockingContext())
            {
                var stock = context.Stocks.Find(storeid);
                return stock;
            }
        }

        public static void UpdateProductStock(Stock stock, int productId, int amount)
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

        public static void AddStock(int storeId, int productId, int amount)
        {
            throw new NotImplementedException();
        }

        public static void ModifyStockBasedOnSale(Sale sale)
        {
            var stock = GetStock(sale.StoreId);
            var amount = (sale.IsReturn ? -1 : 1);
            UpdateProductStock(stock, sale.ProductId, amount);
        }

    }
}