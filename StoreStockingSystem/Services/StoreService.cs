using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Sockets;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class StoreService
    {
        public static Stock GetStoreData(string storeName)
        {
            using (StoreStockingContext ssc = new StoreStockingContext())
            {
                var storeData = ssc.Stocks.Select(y => y).Where(z => z.Store.Name == storeName);
                return storeData.FirstOrDefault();
            }
        }

        public static List<Sale> GetSalesData(DateTime after, DateTime before)
        {
            using (StoreStockingContext ssc = new StoreStockingContext())
            {
                var storeData = from t in ssc.Sales where DateTime.Compare(t.SalesDate, after) > 0 && DateTime.Compare(t.SalesDate, before) < 0 select t;
                //ssc.Sales.SelectMany(t => t).Where(y => DateTime.Compare(y.SalesDate, after) > 0 && DateTime.Compare(y.SalesDate, before) < 0);
                //return storeData.SelectMany(sale => sale.);
            }
        }
    }
}