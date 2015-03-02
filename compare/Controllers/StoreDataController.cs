using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Data;
using PCDSWebsite.Models;

namespace PCDSWebsite.Controllers
{
    public class StoreDataController : Controller
    {
        public ActionResult Index()
        {
           return View("StoreDataList");
        }

        public ActionResult StoreDetails()
        {
            return View("StoreDetails");
        }

    }
}