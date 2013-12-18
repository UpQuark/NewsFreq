using System.Collections.Generic;
using System.Web.Mvc;
using DataNervesUIMVC.Models;

namespace DataNervesUI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult NewsLibrarySearch()
        {
            return View("NewsLibrarySearch");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your app description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
