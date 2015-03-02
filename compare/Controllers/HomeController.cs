using System.Web.Mvc;

namespace PCDSWebsite.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Kunder()
        {
            ViewBag.Message = "Oversigt over kunder";

            return View();
        }

        public ActionResult Kritisk()
        {
            ViewBag.Message = "Kunder som mangler varer";

            return View();
        }
    }
}