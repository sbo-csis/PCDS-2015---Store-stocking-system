using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using PCDSWebsite.Models;

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
            return View();
        }

        public ActionResult StorePerformanceDetails()
        {
            return View();
        }

        public ActionResult CriticalStores()
        {
            return View();
        }

        public ActionResult StorePerformance()
        {
            return View();
        }

        public ActionResult StockUpdate()
        {
            return View();
        }
    }
}