using System.Collections.Generic;
using System.Web.Mvc;
using DataNervesUI.Models;
using NewsLibrarySearch.API;

namespace DataNervesUI.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult NewsLibrarySearch()
        {
            var newsModel = new NewsDataModel()
                                {
                                    Queries = new List<NlQuery>()
                                };
            return View("NewsLibrarySearch", newsModel);
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "";

            return View();
        }
    }
}
