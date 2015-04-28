using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using StoreStockingSystem.Models;
using StoreStockingSystem.Services;

namespace StoreStockingSystem.Data
{
    public static class TestData
    {
        private static readonly string BasePath = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName, @"StoreStockingSystem\Data");
        private static readonly string MerchantPath = Path.Combine(BasePath, "opl_merchants.csv");
        private static readonly string StoresPath = Path.Combine(BasePath, "opl_merchants_stores.csv");
        private static readonly string SalesDataPath = Path.Combine(BasePath, "salesdata.csv");

        public static void BuildData()
        {
            using (var context = new StoreStockingContext())
            {
                InsertFakeDisplayTypes(context);
                InsertMerchants(context);
                InsertStores(context);
                InsertSales(context);

            }
        }

        private static void InsertFakeDisplayTypes(StoreStockingContext context)
        {
            DisplayTypeService.AddDisplayType(new DisplayType
            {
                Capacity = 50,
                Name = "Pap-display"
            }, context);

            DisplayTypeService.AddDisplayType(new DisplayType
            {
                Capacity = 30,
                Name = "Akryl-display"
            }, context);

            context.SaveChanges();
        }

        private static void InsertMerchants(StoreStockingContext context)
        {
            var reader = new StreamReader(File.OpenRead(MerchantPath));
            reader.ReadLine(); // skipping header line

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');

                ChainService.AddChain(new Chain
                {
                    ExternalId = values[0].Replace("\"", string.Empty),
                    Name = values[1].Replace("\"", string.Empty)
                }, context);
            }

            context.SaveChanges();
        }

        private static void InsertStores(StoreStockingContext context)
        {
            var reader = new StreamReader(File.OpenRead(StoresPath));
            reader.ReadLine(); // skipping header line

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');

                var externalChainIdStr = values[0].Replace("\"", string.Empty);

                var chain = (from t in context.Chains
                             where t.ExternalId == externalChainIdStr
                             select t).FirstOrDefault();

                if (chain == null)
                    throw new Exception("Failed to add store: Chain internal ID not found for external chain ID " + externalChainIdStr);

                StoreService.AddStore(new Store
                {
                    ChainId = chain.Id,
                    ExternalId = values[1].Replace("\"", string.Empty),
                    Name = values[2].Replace("\"", string.Empty)
                }, context);
            }
            context.SaveChanges();
        }

        private class StoreIdRelation
        {
            public string ExternalStoreId { get; set; }
            public string ExternalMerchantId { get; set; }
            public int InternalId { get; set; }
        }

        private static void InsertSales(StoreStockingContext context)
        {
            var reader = new StreamReader(File.OpenRead(SalesDataPath));
            reader.ReadLine(); // skipping header line

            var cache = new List<StoreIdRelation>();

            var sales = new List<Sale>();

            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(';');

                var externalStoreId = values[6].Replace("\"", string.Empty);
                var externalMerchantId = values[8].Replace("\"", string.Empty);

                var cacheResult = (from t in cache
                                   where t.ExternalMerchantId == externalMerchantId
                                   && t.ExternalStoreId == externalStoreId
                                   select t).FirstOrDefault();

                if (cacheResult == null) // not found in cache
                {
                    var internalStore = (from t in context.Stores
                                         where t.ExternalId == externalStoreId
                                         && t.Chain.ExternalId == externalMerchantId
                                         select t).FirstOrDefault();

                    if (internalStore == null)
                        throw new Exception("Store with external id " + externalStoreId + " in merchant " + externalMerchantId + " not found!");

                    cacheResult = new StoreIdRelation
                    {
                        ExternalMerchantId = externalMerchantId,
                        ExternalStoreId = externalStoreId,
                        InternalId = internalStore.Id
                    };

                    cache.Add(cacheResult);
                }

                const int displayTypeId = 1;

                var productPrice = decimal.Parse(values[5].Replace("\"", string.Empty));
                var product = GetProduct(productPrice, context);

                StockCheck(cacheResult, product, displayTypeId, context);

                var isReturn = values[2].Replace("\"", string.Empty) != "0" ? true : false;
                var salesDate = DateTime.Parse(values[3].Replace("\"", string.Empty));
                var salesPrice = decimal.Parse(values[5].Replace("\"", string.Empty));

                sales.Add(new Sale
                {
                    DisplayTypeId = displayTypeId,
                    IsReturn = isReturn,
                    Product = product,
                    SalesDate = salesDate,
                    StoreId = cacheResult.InternalId,
                    SalesPrice = (int)salesPrice
                });
            }

            context.SaveChanges();

            SalesService.RegisterSales(sales, context);


        }

        private static void StockCheck(StoreIdRelation cacheResult, Product product, int displayTypeId, StoreStockingContext context)
        {
            var stock = (from t in context.Stocks
                         where t.StoreId == cacheResult.InternalId
                         select t).FirstOrDefault();

            if (stock == null)
            {
                stock = new Stock
                {
                    Capacity = 100000,
                    DisplayTypeId = displayTypeId,
                    StoreId = cacheResult.InternalId,
                    WarningAmountLeft = 10
                };

                context.Stocks.Add(stock);
                context.SaveChanges();
            }

            var productStock = (from t in context.ProductStocks
                                where t.StockId == stock.Id
                                &&    t.ProductId == product.Id
                                select t).FirstOrDefault();

            if (productStock == null)
            {
                productStock = new ProductStock
                {
                    Amount = 10000,
                    Product = product,
                    StockId = stock.Id
                };

                context.ProductStocks.Add(productStock);
                context.SaveChanges();
            }

            StockService.GetStock(stock.Id, context);
        }

        private static Product GetProduct(decimal price, StoreStockingContext context)
        {
            var product = (from t in context.Products
                           where t.Price == price
                           select t).FirstOrDefault();

            if (product == null)
            {
                product = new Product
                {
                    Name = RandomString(5),
                    Price = (int)price
                };
                ProductService.NewProduct(product);
            }

            return product;
        }


        private static readonly Random Rng = new Random();
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

        private static string RandomString(int size)
        {
            var buffer = new char[size];

            for (int i = 0; i < size; i++)
            {
                buffer[i] = Chars[Rng.Next(Chars.Length)];
            }
            return new string(buffer);
        }
    }
}