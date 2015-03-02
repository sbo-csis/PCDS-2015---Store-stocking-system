using System.Web.Mvc;
using System.Data;

namespace PCDSWebsite.Controllers
{
    public class StoreDataController : Controller
    {
        public ActionResult Index()
        {
            return View("StoreDataList");
        }

        public static DataTable MakeMockTable()
        {
            
        }
    }
}