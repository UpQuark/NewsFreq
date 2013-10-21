using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewsLibrarySearch;

namespace DataNervesUIMVC.Models
{
    public class QueryModel
    {
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public String DateString { get; set; }
        public String SearchString { get; set; }
        public string SearchTargetId { get; set; }
        public IEnumerable<SelectListItem> SearchTargets { get; set; }
        public DateTime CreatedDate { get; set; }
        public int Count { get; set; }
    }
}