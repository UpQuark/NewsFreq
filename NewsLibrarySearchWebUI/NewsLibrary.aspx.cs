using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewsLibrarySearch;
using NewsLibrarySearch.API;

namespace NewsLibrarySearchUI
{
    public partial class NewsLibrary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        [WebMethod]
        public static Object Query(DateTime dateFrom, DateTime dateTo, String dateString, String searchString, String searchTarget)
        {
            return new NlQuery(dateFrom, dateTo, dateString, searchString, searchTarget, DateTime.Now);
        }
    }
}