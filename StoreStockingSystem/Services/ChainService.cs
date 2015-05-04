using System.Collections.Generic;
using System.Linq;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Services
{
    /// <summary>
    /// Class for fetching and manipulating store chains. Should be used for getting store chains and adding new store chains to database.
    /// </summary>
    public class ChainService
    {
        /// <summary>
        /// Add new store chain object to database. Returns the chain object after inserting.
        /// </summary>
        /// <param name="chain">Store chain object.</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
        public static Chain AddChain(Chain chain, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            context.Chains.Add(chain);
            context.SaveChanges();
            return chain;
        }

        /// <summary>
        /// Creates new chain object and adds it to database. Returns the chain object after inserting.
        /// </summary>
        /// <param name="name">Chain name.</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
        public static Chain AddChain(string name, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return AddChain(new Chain { Name = name }, context);
        }

        /// <summary>
        /// Gets chain object based on chain ID.
        /// </summary>
        /// <param name="chainId">Chain ID.</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
        public static Chain GetChain(int chainId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return (from t in context.Chains
                    where t.Id == chainId
                    select t).FirstOrDefault();
        }

        /// <summary>
        /// Gets all stores belonging to a chain.
        /// </summary>
        /// <param name="chain">Chain object.</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
        public static List<Store> GetChainStores(Chain chain, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return (from t in context.Stores
                    where t.Chain.Id == chain.Id
                    select t).ToList();
        }

        /// <summary>
        /// Gets all stores belonging to a chain.
        /// </summary>
        /// <param name="chainId">Chain ID.</param>
        /// <param name="context">Optional database context.</param>
        /// <returns></returns>
        public static List<Store> GetChainStores(int chainId, StoreStockingContext context = null)
        {
            if (context == null)
                context = new StoreStockingContext();

            return (from t in context.Stores
                    where t.Chain.Id == chainId
                    select t).ToList();
        }

        /// <summary>
        /// Removes a chain.
        /// </summary>
        /// <param name="chain">Chain object to remove.</param>
        /// <param name="context">Optional database context.</param>
        public static void RemoveChain(Chain chain, StoreStockingContext context)
        {
            if (context == null)
                context = new StoreStockingContext();

            context.Chains.Remove(chain);
            context.SaveChanges();
        }

    }
}