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

        public static Stock NewStock(Stock newStock)
        {
            using (var context = new StoreStockingContext())
            {
                if (newStock.Store != null)
                    context.Stores.Attach(newStock.Store);
                if (newStock.DisplayType != null)
                    context.DisplayTypes.Attach(newStock.DisplayType);

                context.Stocks.Add(newStock);
                context.SaveChanges();
                return newStock;
            }
        }

        public static Stock NewStock(Store store, DisplayType displayType, int? capacity, int? warningPercentage)
        {
            return NewStock(new Stock
            {
                Capacity = capacity ?? displayType.Capacity, // Default capacity equal to displaytypes standard capacity
                DisplayType = displayType,
                DisplayTypeId = displayType.Id,
                Store = store,
                StoreId = store.Id,
                WarningPercentageStockLeft = warningPercentage ?? 10 // Default warning at 10%
            });
        }

        public static Stock NewStock(int storeId, int displayTypeId, int? capacity, int? warningPercentage)
        {
            var store = StoreService.GetStore(storeId);
            var displayType = DisplayTypeService.GetDisplayType(displayTypeId);

            return NewStock(new Stock
            {
                Capacity = capacity ?? displayType.Capacity, // Default capacity equal to displaytypes standard capacity
                DisplayType = displayType,
                Store = store,
                WarningPercentageStockLeft = warningPercentage ?? 10 // Default warning at 10%
            });
        }

        public static Stock GetStock(int stockId)
        {
            using (var context = new StoreStockingContext())
            {
                return (from t in context.Stocks
                        where t.Id == stockId
                        select t).FirstOrDefault();

            }
        }

        public static Stock GetStock(Store store, int displayTypeId)
        {
            using (var context = new StoreStockingContext())
            {
                context.Stores.Attach(store);
                var stock = (from t in context.Stocks
                             where t.Store.Id == store.Id
                                && t.DisplayType.Id == displayTypeId
                             select t).FirstOrDefault();

                if (stock == null)
                    throw new ArgumentException("Could not find stock for store id: " + store.Id + " and display-type id " + displayTypeId);
                
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

        public static ProductStock GetProductStock(Stock stock, Product product)
        {
            using (var context = new StoreStockingContext())
            {
                context.Stocks.Attach(stock);
                context.Products.Attach(product);
                var productStock = (from t in context.ProductStocks
                                    where t.Stock.Id == stock.Id && t.Product.Id == product.Id
                                    select t).FirstOrDefault();
                return productStock;
            }
        }

        public static ProductStock GetProductStock(int stockId, int productId)
        {
            return GetProductStock(StockService.GetStock(stockId), ProductService.GetProduct(productId));
        }

        public static List<ProductStock> GetAllProductStocks(int stockId)
        {
            using (var context = new StoreStockingContext())
            {
                var productStock = (from t in context.ProductStocks
                                    where t.Stock.Id == stockId
                                    select t).ToList();
                return productStock;
            }
        }

        public static List<Stock> GetAllStocks()
        {
            using (var context = new StoreStockingContext())
            {
                List<Stock> stocks = (from t in context.Stocks
                             select t).ToList();
                return stocks;
            }
        }

        public static void AddProductToStock(Stock stock, Product product, int amount)
        {
            using (var context = new StoreStockingContext())
            {
                context.Products.Attach(product);
                
                var productStock = (stock.ProductStock != null) ? (from t in stock.ProductStock
                                                                   where t.ProductId == product.Id
                                                                   select t).FirstOrDefault() 
                                                                : null;

                if (productStock == null) //Adding new product to stock
                {
                    context.ProductStocks.Add((new ProductStock()
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
        }

        public static void AddProductToStock(int stockId, int productId, int amount)
        {
            using (var context = new StoreStockingContext())
            {
                var stock = context.Stocks.Find(stockId);
                if(stock == null)
                    throw new ArgumentException("Failed adding product to stock. Could not find stock id: " + stockId);

                var product = context.Products.Find(productId);
                if (product == null)
                    throw new ArgumentException("Failed adding product to stock. Could not find product id: " + productId);

                AddProductToStock(stock, product, amount);
            }
        }

        public static void ModifyStockBasedOnSale(Sale sale)
        {
            var productStock = GetProductStockBasedOnSalesData(sale.DisplayType, sale.Store, sale.Product);
            var amount = (sale.IsReturn ? 1 : -1); // add 1 back to stock if return, otherwise remove 1 from stock.
            AddProductToStock(productStock.StockId, sale.Product.Id, amount);
        }

        private static ProductStock GetProductStockBasedOnSalesData(DisplayType displayType, Store store, Product product)
        {
            using (var context = new StoreStockingContext())
            {
                context.DisplayTypes.Attach(displayType);
                context.Stores.Attach(store);
                context.Products.Attach(product);

                var stock = (from t in context.Stocks
                             where t.Store.Id == store.Id && t.DisplayType.Id == displayType.Id
                             select t).FirstOrDefault();

                if(stock == null)
                    throw new ArgumentException("No stock for store \"" + store.Name + "\" with a display type: \"" + displayType.Name +"\"");

                return (from t in stock.ProductStock
                        where t.ProductId == product.Id
                        select t).FirstOrDefault();
            }
        }

        public static void RemoveProductFromStock(Stock stock, Product product)
        {
            using (var context = new StoreStockingContext())
            {
                context.Stocks.Attach(stock);
                context.Products.Attach(product);

                var productStock = (from t in context.ProductStocks
                                    where t.Stock == stock && t.Product == product
                                    select t).FirstOrDefault();

                if (productStock == null)
                    throw new ArgumentException("Could not find any product: \"" + product + "\" in stock id " + stock.Id);

                context.ProductStocks.Remove(productStock);
                context.SaveChanges();
            }
        }

        public static void RemoveProductFromStock(int stockId, int productId)
        {
           RemoveProductFromStock(StockService.GetStock(stockId), ProductService.GetProduct(productId));
        }
    }
}