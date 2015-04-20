using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    public class ChainService
    {
        public static Chain AddChain(Chain chain, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            context.Chains.Add(chain);
            context.SaveChanges();
            return chain;
        }

        public static Chain AddChain(string name, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return AddChain(new Chain { Name = name }, context);
        }

        public static Chain GetChain(int chainId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return (from t in context.Chains
                    where t.Id == chainId
                    select t).FirstOrDefault();
        }

        public static List<Store> GetChainStores(Chain chain, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return (from t in context.Stores
                    where t.Chain == chain
                    select t).ToList();
        }

        public static List<Store> GetChainStores(int chainId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return (from t in context.Stores
                    where t.Chain.Id == chainId
                    select t).ToList();
        }

        public static void RemoveChain(Chain chain, StoreStockingContext context)
        {
            if (context == null)
                context = new StoreStockingContext();

            context.Chains.Remove(chain);
            context.SaveChanges();
        }

        public static List<Sale> GetChainSales(Chain chain, DateTime fromDate, DateTime toDate, StoreStockingContext context)
        {
            if (context == null)
                context = new StoreStockingContext();

            // Get stores in chain
            var chainStores = ChainService.GetChainStores(chain, context);

            // Get accumulated sales for the chain stores 
            List<Sale> chainSales = new List<Sale>(); 
            foreach (Store store in chainStores)
            {
                chainSales.AddRange(SalesService.GetSales(store.Id, fromDate, toDate, context));
            }

            return chainSales;
        }
    }
}