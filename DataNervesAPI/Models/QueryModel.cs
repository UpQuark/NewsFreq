using System;
using System.Collections.Generic;
using System.Web.Mvc;
using NewsLibrarySearch.API;
using System.Collections;

namespace DataNervesAPI.Models
{


    public class QueryModel
    {
        public List<NlQuery> Queries { get; set; }
        //public NlQuery Query { get; set; }
        public string SearchType { get; set; }
    }
}