using NUnit.Framework;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Test
{
    [TestFixture]
    public class BuildMockData
    {
        
        [Test]
        [Ignore] // Ignoring to avoid running on CI.
        public void InsertData()
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


            Assert.IsTrue(1 == 1);
        }
    }
}