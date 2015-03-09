using System.Linq;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class StoreService
    {
        public static void GetStoreData(string storeName)
        {
            using (StoreStockingContext ssc = new StoreStockingContext())
            {
                var x = ssc.ProductStocks.Select(y => y.Stock).Where(z => z.Store.Name == storeName);
            }
        }
    }
}