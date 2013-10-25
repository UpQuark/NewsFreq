using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NewsLibrarySearch;

namespace DataNervesUIMVC.Models
{
    public class litem
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class QueryModel
    {


        public List<litem> _list;

        
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public String DateString { get; set; }
        public String SearchString { get; set; }
        public string SearchTargetId { get; set; }
        public IEnumerable<SelectListItem> SearchTargets
        {
            get { return new SelectList(_list, "Id", "Name"); }
        }
        public DateTime CreatedDate { get; set; }
        public int Count { get; set; }
    }
}