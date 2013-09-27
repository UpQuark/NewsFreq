using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Services;

namespace NewsLibrarySearch.API
{
    class NlWeb
    {
        [WebMethod]
        public static Object Query(DateTime dateFrom, DateTime dateTo, String dateString, String searchString, String searchTarget)
        {
            return new NlQuery(dateFrom, dateTo, dateString, searchString, searchTarget, DateTime.Now);
        }
    }
}
