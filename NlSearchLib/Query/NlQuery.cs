using System;

namespace NewsLibrarySearch.Query
{
    public class NlQuery
    {
        private NlSearchData _searchData;


        public NlQuery(DateTime dateFrom, DateTime dateTo, string searchTerm, string fieldTarget, DateTime now) 
        {
          //  _searchData
        }

        public NlQuery(NlSearchData n)
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
