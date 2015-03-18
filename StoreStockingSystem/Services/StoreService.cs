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

        

        public static List<ProductStock> GetStoreProducts(int storeId)
        {
            using (var context = new StoreStockingContext())
            {
                List<ProductStock> productList = (from t in context.ProductStocks
                             where t.Stock.Store.Id == storeId
                             select t).ToList();
                return productList;
            }
        }

        // Returns new store id.
        public static int AddStore(Store store) 
        {
            using (var context = new StoreStockingContext())
            {
                context.Stores.Add(store);
                context.SaveChanges();
                return store.Id;
            }
        }

        public static void AddStore(string storeName)
        {
            AddStore(new Store{Name = storeName});
        }

        public static void RenameStore(Store store, string newName)
        {
            using (var context = new StoreStockingContext())
            {
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
        }

        public static void RenameStore(int storeId, string newName)
        {
            using (var context = new StoreStockingContext())
            {
                var store = context.Stores.Find(storeId);

                if(store == null)
                    throw new Exception("Could not find store with id " + storeId);

                RenameStore(store, newName);
            }
        }

        public static void AssignNewPersonToStore(Store store, int personId) 
        {
            //TODO: add person class and then replace personId with person class as argument.
            //TODO: create person table and create foreign keys, then do logic here.
            throw new NotImplementedException();
        }
    }
}