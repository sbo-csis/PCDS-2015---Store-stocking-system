using System.Linq;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class StoreService
    {
        public static Store GetStore(int storeId)
        {
            using (var context = new StoreStockingContext())
            {
                var store = (from t in context.Stores
                             where t.Id == storeId
                             select t).FirstOrDefault();

                return store;
            }
        }

        public static void GetStoreData(string storeName)
        {
            using (StoreStockingContext ssc = new StoreStockingContext())
            {
                var x = ssc.ProductStocks.Select(y => y.Stock).Where(z => z.Store.Name == storeName);
            }
        }
    }
}