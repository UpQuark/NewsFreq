using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NewsLibrarySearch.Query
{
    public class NlQuery
    {

        public NlQuery(DateTime dateFrom, DateTime dateTo, string searchTerm, string fieldTarget, DateTime now) 
        {

        }

        public NlQuery(NlSearchData n)
        {
            
        }

        public static NlSearchData MakeRequest(DateTime dateFrom, DateTime dateTo, string searchTerm, string fieldTarget, DateTime now)
        {
            //Create a new search object
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
                Time = now,
            };

            return s;
        }
    }
}
