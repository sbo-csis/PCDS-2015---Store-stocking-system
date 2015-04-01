using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class StoreService
    {
        public static Store GetStore(int storeId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();
            
            var store = (from t in context.Stores
                            where t.Id == storeId
                            select t)
                            .Include(t => t.SalesPerson)
                            .Include(t => t.Chain)
                            .FirstOrDefault();

            return store;
        }

        public static IEnumerable<Store> GetStores(StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var stores = (from t in context.Stores
                         select t);

            return stores;
        }

        public static IEnumerable<ProductStock> GetStoreProducts(int storeId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return (from t in context.ProductStocks
                    where t.Stock.Store.Id == storeId
                    select t);
        }

        // Returns new store id.
        public static Store AddStore(Store store, StoreStockingContext context = null) 
        {
            if (context == null)
                context = new StoreStockingContext();

            context.Stores.Add(store);
            context.SaveChanges();
            return store;
        }

        public static Store AddStore(string storeName, StoreStockingContext context = null)
        {
            return AddStore(new Store {Name = storeName}, context);
        }

        public static void RenameStore(Store store, string newName, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            try
            {
                context.Stores.Find(store.Id).Name = newName;
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception("Could not rename store id: " + store.Id, ex);
            }

        }

        public static void RenameStore(int storeId, string newName, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var store = context.Stores.Find(storeId);

            if(store == null)
                throw new Exception("Could not find store with id " + storeId);

            RenameStore(store, newName, context);
        }

        public static void RemoveStore(int storeid, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var store = GetStore(storeid, context);

            RemoveStore(store, context);
        }

        public static void RemoveStore(Store store, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            context.Stores.Remove(store);
            context.SaveChanges();
        }

        public static void AssignNewPersonToStore(Store store, int personId) 
        {
            //TODO: add person class and then replace personId with person class as argument.
            //TODO: create person table and create foreign keys, then do logic here.
            throw new NotImplementedException();
        }

        public static IEnumerable<DisplayType> GetStoreDisplays(int storeId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var displayTypes = (from t in context.Stocks
                         where t.Id == storeId
                         select t.DisplayType);

            return displayTypes;
        }

        public static int GetStoreCapacity(int storeId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var capacity = (from t in context.Stocks
                                where t.Id == storeId
                                select t.Capacity).FirstOrDefault();

            return capacity;
        }
    }
}