using System.Web.Mvc;
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
            return View(SalesService.StorePerformanceDetails(99));
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