using NUnit.Framework;
using StoreStockingSystem.Models;

namespace StoreStockingSystem.Test.Tests
{
    [TestFixture]
    public class StoreServiceTest
    {
        [Test]
        [Ignore]
        public void make_and_delete_store()
        {
            using (var context = new StoreStockingContext())
            {
                // Dummy salesperson
                var salesPerson = new SalesPerson { Name = "Ole Jensen", Id = 1 };
                // Add salesPerson if it does not exist
                if (context.SalesPersons.Find(salesPerson.Id) != null)
                {
                    context.SalesPersons.Add(salesPerson);
                }
                // Dummy store
                var store = new Store
                {
                    Id = 1,
                    Name = "Fona Amager",
                    SalesPerson = salesPerson
                };
                // Add store if it does not exist
                if (context.Stores.Find(store.Id) != null)
                {
                    context.Stores.Add(store);
                }
                context.SaveChanges();

                // Test that the store is added 
                Assert.NotNull(context.Stores.Find(store.Id));

                // Delete the store from the context
                context.Stores.Remove(store);
                context.SaveChanges();
                // Test that it has been deleted 
                Assert.Null(context.Stores.Find(store.Id));
            }
        }
    }
}