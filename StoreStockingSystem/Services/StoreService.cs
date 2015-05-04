using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public static class StoreService
    {
        /// <summary>
        /// Gets populated store object.
        /// </summary>
        /// <param name="storeId">ID for store that will be populated.</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets all existing stores.
        /// </summary>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
        public static IEnumerable<Store> GetStores(StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var stores = (from t in context.Stores
                         select t);

            return stores;
        }

        /// <summary>
        /// Get stocks for each product in a store.
        /// </summary>
        /// <param name="storeId">ID for store.</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
        public static IEnumerable<ProductStock> GetStoreProducts(int storeId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return (from t in context.ProductStocks
                    where t.Stock.Store.Id == storeId
                    select t);
        }

        /// <summary>
        /// Create new store.
        /// </summary>
        /// <param name="store">Populated store object.</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
        public static Store AddStore(Store store, StoreStockingContext context = null) 
        {
            if (context == null)
                context = new StoreStockingContext();

            context.Stores.Add(store);
            context.SaveChanges();
            return store;
        }

        /// <summary>
        /// Create a new simple store. Will not have any parameters set, except for the store name.
        /// </summary>
        /// <param name="storeName">Store name.</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
        public static Store AddStore(string storeName, StoreStockingContext context = null)
        {
            return AddStore(new Store {Name = storeName}, context);
        }

        /// <summary>
        /// Rename a store.
        /// </summary>
        /// <param name="store">Store object to rename.</param>
        /// <param name="newName">New store name.</param>
        /// <param name="context">Optinal database context.</param>
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

        /// <summary>
        /// Rename a store.
        /// </summary>
        /// <param name="storeId">Store ID to rename.</param>
        /// <param name="newName">New store name.</param>
        /// <param name="context">Optinal database context.</param>
        public static void RenameStore(int storeId, string newName, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var store = context.Stores.Find(storeId);

            if(store == null)
                throw new Exception("Could not find store with id " + storeId);

            RenameStore(store, newName, context);
        }

        /// <summary>
        /// Remove a store from the database.
        /// </summary>
        /// <param name="storeid">Store ID to remove.</param>
        /// <param name="context">Optional database context.</param>
        public static void RemoveStore(int storeid, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var store = GetStore(storeid, context);

            RemoveStore(store, context);
        }

        /// <summary>
        /// Remove a store from the database.
        /// </summary>
        /// <param name="store">Store object to remove.</param>
        /// <param name="context">Optional database context.</param>
        public static void RemoveStore(Store store, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            context.Stores.Remove(store);
            context.SaveChanges();
        }

        /// <summary>
        /// Assign sales person to a store.
        /// </summary>
        /// <param name="store">Store object.</param>
        /// <param name="personId">Sales person ID to assign to store.</param>
        public static void AssignNewPersonToStore(Store store, int personId) 
        {
            //TODO: add person class and then replace personId with person class as argument.
            //TODO: create person table and create foreign keys, then do logic here.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get all displays currently used in store.
        /// </summary>
        /// <param name="storeId">Store ID to get displays for.</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
        public static IEnumerable<DisplayType> GetStoreDisplays(int storeId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            var displayTypes = (from t in context.Stocks
                         where t.Id == storeId
                         select t.DisplayType);

            return displayTypes;
        }

        /// <summary>
        /// Get stores total product capacity.
        /// </summary>
        /// <param name="storeId">Store ID to get capacity for.</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
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