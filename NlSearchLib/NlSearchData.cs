using System;

namespace NewsLibrarySearch
{
    /// <summary>
    /// NewsLibrary search data. Both sent and recieved.
    /// </summary>
    public class NlSearchData
    {
        public NlSearchData()
        {
            
        }
 
        public NlSearchData(DateTime dateFrom, DateTime dateTo, DateTime timeOfQuery, string searchTerm, string fieldTarget, int count = 0)
        {
            DateFrom = dateFrom;
            DateTo = dateTo;
            TimeofQuery = timeOfQuery;
            SearchTerm = searchTerm;
            FieldTarget = fieldTarget;
            Count = count;
        }

    public string DateString { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime TimeofQuery { get; set; }
        public string SearchTerm { get; set; }
        public string FieldTarget { get; set; }
        public int Count { get; set; }

        public string DateFromString
        {
            get { return DateFrom.ToString("MMMM dd, yyyy"); }
        }

        public string DateToString
        {
            get { return DateTo.ToString("MMMM dd, yyyy"); }
        }
    }
}