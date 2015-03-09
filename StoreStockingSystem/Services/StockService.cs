using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class StockService
    {
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
                Capacity = capacity ?? 9999,
                DisplayType = displayType,
                Store = store,
                WarningPercentageStockLeft = warningPercentage ?? 0
            });
        }

        public static void AddStock(int storeId, int productId, int amount)
        {
            
        }

    }
}