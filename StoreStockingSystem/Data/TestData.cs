using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Data
{
    public static class TestData
    {
        private const string MerchantPath = "opl_merchants.csv";
        private const string StoresPath = "opl_merchants_stores.csv";
        private const string SalesDataPath = "salesdata.csv";

        public static void BuildData()
        {
            InsertMerchants();
            InsertStores();
            InsertSales();
        }

        private static void InsertMerchants()
        {
            var reader = new StreamReader(File.OpenRead(MerchantPath));

            using (var context = new StoreStockingContext())
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    context.Chains.Add(new Chain
                    {
                        ExternalId = values[0],
                        Name = values[1]
                    });
                }
                context.SaveChanges();
            }
        }

        private static void InsertStores()
        {
            var reader = new StreamReader(File.OpenRead(StoresPath));

            using (var context = new StoreStockingContext())
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    context.Stores.Add(new Store
                    {
                        ChainId = int.Parse(values[0]),
                        ExternalId = values[1],
                        Name = values[2]
                    });
                }
                context.SaveChanges();
            }
        }

        private class StoreIdRelation
        {
            public string ExternalStoreId { get; set; }
            public string ExternalMerchantId { get; set; }
            public int InternalId { get; set; }
        }

        private static void InsertSales()
        {
            var reader = new StreamReader(File.OpenRead(SalesDataPath));

            var cache = new List<StoreIdRelation>();

            using (var context = new StoreStockingContext())
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(';');

                    var storeId = values[5];
                    var merchantId = values[7];

                    var cacheResult = (from t in cache
                                      where t.ExternalMerchantId == merchantId
                                      && t.ExternalStoreId == storeId
                                      select t).FirstOrDefault();

                    if (cacheResult == null) // not found in cache
                    {
                        var internalStore = (from t in context.Stores
                                             where t.ExternalId == storeId
                                             && t.Chain.ExternalId == merchantId
                                             select t).FirstOrDefault();

                        if (internalStore != null)
                            throw new Exception("Store with external id " + storeId + " in merchant " + merchantId + " not found!");

                        cacheResult = new StoreIdRelation
                        {
                            ExternalMerchantId = merchantId,
                            ExternalStoreId = storeId,
                            InternalId = internalStore.Id
                        };

                        cache.Add(cacheResult);
                    }

                    context.Sales.Add(new Sale
                    {

                    });
                }
                context.SaveChanges();
            }
        }
    }
}