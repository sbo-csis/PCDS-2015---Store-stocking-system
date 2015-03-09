using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NUnit.Framework;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Tests
{
    [TestFixture]
    public class RandomTest2
    {
        
        [Test]
        [Ignore]
        public void AlwaysFails()
        {
            using (var context = new StoreStockingContext())
            {
                var store = new Store
                {
                    Name = "hello"
                };

                context.Stores.Add(store);
                context.SaveChanges();
            }


            Assert.IsTrue(1 == 2);
        }
    }
}