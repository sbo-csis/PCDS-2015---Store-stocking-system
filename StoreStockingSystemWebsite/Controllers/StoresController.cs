using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using PCDSWebsite.Models;
using StoreStockingSystem.Services;

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
            var model = StockService.GetStock(store);

            return View(model);
        }

        public ActionResult StorePerformanceDetails()
        {
            return View();
        }

        public ActionResult CriticalStores()
        {
            var model = StockService.GetLowStocks();

            return View(model);
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