using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
using StoreStockingSystem.Models;
using StoreStockingSystem.Services;

namespace StoreStockingSystem.Test.Services
{
    [TestFixture]
    public class ChainServiceTest
    {
        [Test]
        public void can_make_chain_add_store_and_delete_chain()
        {
            using (var context = new StoreStockingContext())
            {
                var chain = ChainService.AddChain(new Chain
                {
                    Name = "Fona (Unit Test)"
                }, context);

                context.SaveChanges();

                var foundChain = ChainService.GetChain(chain.Id, context);

                Assert.AreEqual(chain,foundChain);

                var store = StoreService.AddStore(new Store
                {
                    Name = "Fona Valby (Unit Test)",
                    ChainId = chain.Id
                }, context);

                var chainStoreListAfterInsert = ChainService.GetChainStores(chain, context);
                Assert.AreNotEqual(chainStoreListAfterInsert, null);

                ChainService.RemoveChain(chain, context);

                var chainAfterDelete = ChainService.GetChain(chain.Id, context);

                Assert.AreEqual(chainAfterDelete, null);
            }   
        }
    }
}