using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using PCDSWebsite.Models;
using StoreStockingSystem.Models;
using StoreStockingSystem.Services;
using Stock = StoreStockingSystem.Models.Stock;

namespace PCDSWebsite.Controllers
{
    public class StoresController : Controller
    {
        public ActionResult ChannelsPerformance()
        {
            return View();
        }

        public ActionResult Stores()
        {
            var model = StoreService.GetStores();

            return View(model);
        }

        public ActionResult StoreDetails(int id = 0)
        {
            var store = StoreService.GetStore(id);
            var stocks = StockService.GetStocks(store);
            foreach (var stock in stocks)
            {
                StockService.GetStockWithEmptyDate(stock);
            }

            return View(stocks);
        }

        //Pass parameter id
        public ActionResult StorePerformanceDetails()
        {

            return View();

        }

        public ActionResult CriticalStores()
        {
            var model = StockService.GetStocksNeedingRefilling();

            return View(model);
        }

        [HttpPost] //TODO: Refactor this method, was completed in a hurry before sprint end.
        public void CriticalStores(List<Stock> stocks)
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

        public ActionResult StorePerformance()
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