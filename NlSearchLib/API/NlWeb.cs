using System;
using System.Web.Services;

namespace NewsLibrarySearch.API
{
    class NlWeb
    {
        [WebMethod]
        public static NlQuery Query(DateTime dateFrom, DateTime dateTo, String dateString, String searchString, String searchTarget)
        {
            return new NlQuery(dateFrom, dateTo, dateString, searchString, searchTarget, DateTime.Now);
        }
    }
}
