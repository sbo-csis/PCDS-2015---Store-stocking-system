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
    }
}