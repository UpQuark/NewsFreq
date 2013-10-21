using System;
using System.Net.Http;
using System.Web.Http;
using NewsLibrarySearch.API;

namespace DataNervesUIMVC.Controllers
{
    public class NewsLibraryController : ApiController
    {
        public int GetCount(string id)
        {
            var queryValues = Request.RequestUri.ParseQueryString();
            var query = new NlQuery
            {
                DateFrom = DateTime.Parse(queryValues[0]),
                DateTo = DateTime.Parse(queryValues[1]),
                DateString = queryValues[2],
                SearchString = queryValues[3],
                SearchTarget = queryValues[4],
            };
            query.SendQuery();
            return query.Count;
        }
    }
}