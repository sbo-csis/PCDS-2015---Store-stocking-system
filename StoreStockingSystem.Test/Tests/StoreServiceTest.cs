using System;
using NUnit.Framework;
using StoreStockingSystem.Models;
using StoreStockingSystem.Services;

namespace StoreStockingSystem.Test.Tests
{
    [TestFixture]
    public class StoreServiceTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void can_make_store_add_stock_to_it_and_delete_store()
        {
            using (var context = new StoreStockingContext())
            {
                var store = StoreService.AddStore(new Store()
                {
                    Name = "Bilka (Unit Test)"
                }, context);

                var foundStore = StoreService.GetStore(store.Id, context);

                Assert.AreEqual(store, foundStore);

                var displayType = DisplayTypeService.AddDisplayType(new DisplayType()
                                                                    {
                                                                        Capacity = 123,
                                                                        Name = "Jern display (Unit Test)"
                                                                    }, context);

                var stock = StockService.NewStock(new Stock()
                                                    {
                                                        Capacity = 100,
                                                        DisplayTypeId = displayType.Id,
                                                        StoreId = store.Id,
                                                        WarningPercentageStockLeft = 10
                                                    }, context);

                var stockAfterInsert = StockService.GetStock(store, displayType.Id);
                Assert.AreEqual(stockAfterInsert.Id, stock.Id);

                StoreService.RemoveStore(store.Id, context);

                var storeAfterDeletion = StoreService.GetStore(store.Id, context);
                Assert.AreEqual(storeAfterDeletion,null);

                var displayTypeAfterDeletion = DisplayTypeService.GetDisplayType(displayType.Id, context);

                Assert.AreEqual(displayType.Id, displayTypeAfterDeletion.Id);

                StockService.GetStock(store, displayType.Id, context); //Exception happens here
            }
        }
    }
}