using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NewsLibrarySearch.API;

namespace DataNervesAPI.Models
{


    public class QueryModel
    {
        public NlQuery Query { get; set; }
        public string SearchType { get; set; }
    }
}