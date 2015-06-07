using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.Ajax.Utilities;
using PCDSWebsite.Models;
using StoreStockingSystem.Services;
using Stock = StoreStockingSystem.Models.Stock;

namespace PCDSWebsite.Controllers
{
    public class StoresController : Controller
    {
        public ActionResult Index()
        {
            var model = StoreService.GetStores();

            return this.View(model);
        }

        [HttpGet]
        public ActionResult StoreDetails(int id = 0)
        {
            var store = StoreService.GetStore(id);
            var stocks = StockService.GetStocks(store);
            var storePerformance = SalesService.GetStorePerformanceDetails(id);

            var model = new StoreViewModel
            {
                Store = store,
                Stocks = stocks,
                Performance = storePerformance
            };

            return View(model);
        }

        public ActionResult ChainDetails(DateTime startTime, DateTime endTime, int id = 0)
        {
            var chain = ChainService.GetChain(id);
            var stores = ChainService.GetChainStores(id);
            var performance = ChainPerformance(startTime, endTime, id);
            

            var model = new ChainViewModel
            {
                Chain = chain,
                Stores = stores,
                Performance = performance
            };

            return View(model);
        }

        public ChainPerformanceModel ChainPerformance(DateTime startTime, DateTime endTime, int id = 0)
        {
         
            var values = SalesService.GetMonthlyChainPerformance(id, startTime, endTime, null);
            var chainPerformance = new ChainPerformanceModel
            {
                StartTime = startTime,
                EndTime = endTime,
                Values = values

            };
            
            
            return chainPerformance;
        }

        public ActionResult ChainsOverview()
        {
            var model = ChainService.GetChains();

            return View(model);
        }

        public ActionResult StockList()
        {
            var model = StockService.GetStocksNeedingRefilling();

            return View(model);
        }

        public ActionResult Stores()
        {
            var model = StoreService.GetStores();

            return View(model);
        }

        [HttpPost] //TODO: Refactor this method, was completed in a hurry before sprint end.
        public void StockList(List<Stock> stocks)
        {
            var productsRefill = new List<RefillEntry>(); //TODO: Refactor into seperate class. Currently holds a tuple <product, displayType, amountToBringForRefill>

            foreach (var stock in stocks)
            {
                foreach (var productStock in stock.ProductStocks)
                {
                    if (productStock.CurrentAmount < productStock.Capacity)
                    {
                        var productMissing = 0;

                        if (productStock.CurrentAmount >= 0 && productStock.Capacity >= productStock.CurrentAmount)
                        {
                            productMissing = productStock.Capacity - productStock.CurrentAmount;
                        }
                        else
                        {
                            productMissing = productStock.Capacity + Math.Abs(productStock.CurrentAmount);
                        }

                        var refillEntry = productsRefill.FirstOrDefault(p => p.Product.Id == productStock.Product.Id && p.DisplayType.Id == stock.DisplayTypeId);

                        if (refillEntry == null)
                        {
                            productsRefill.Add(new RefillEntry
                            {
                                Product = productStock.Product,
                                DisplayType = stock.DisplayType,
                                RefillCount = productMissing
                            });
                        }
                        else
                        {
                            refillEntry.RefillCount += productMissing;
                        }
                    }
                }
            }

            Refill(productsRefill);
        }

        public ActionResult Refill(List<RefillEntry> StocksToRefill )
        {
            return View(StocksToRefill);
        }

        public ActionResult Alerts()
        {
            return View();
        }

        public ActionResult StockUpdate()
        {
            return View();
        }
        public ActionResult SalesStockUpdate()
        {
            return View();
        }
    }
}