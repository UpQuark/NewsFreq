using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using NewsLibrarySearch;

namespace NewsLibrarySearchUI
{
    public partial class NewsLibrary : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        public static NlSearchData MakeRequest(DateTime dateFrom, DateTime dateTo, string searchTerm, string fieldTarget, DateTime now)
        {
            NlAdvancedSearch search = new NlAdvancedSearch();

            string dateString = dateFrom.ToString("MMMM dd, yyyy") + " to " + dateTo.ToString("MMMM dd, yyyy");

            //Create search result to populate label
            NlSearchData s = new NlSearchData
            {
                DateFrom = dateFrom,
                DateTo = dateTo,
                SearchTerm = searchTerm,
                DateString = dateString,
                FieldTarget = NlSearchTargets.Key(fieldTarget),
                Count = search.SendSearchRequest(dateString, searchTerm, fieldTarget),
                TimeofQuery = now,
            };

            return s;
        }
    }
}