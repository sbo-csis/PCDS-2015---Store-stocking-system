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
                    where t.Chain.Id == chain.Id
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

    }
}