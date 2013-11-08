using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DataNervesUIMVC.Models;

namespace DataNervesUIMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "";

            var li = new List<ListItem>();
            li.Add(new ListItem
            {
                Id = 6,
                Name = ""
            }
            );

            var queryBase = new QueryModel
            {
                _list = li,
               // DateFrom = null,
               // DateTo = null
            
            };


            return View(queryBase);
        }

        public ActionResult Search()
        {




            ViewBag.Message = "";

            return View("Index");
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
