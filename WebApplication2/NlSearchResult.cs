using System;

namespace NewsLibrarySearchUI
{
    public class NlSearchResult
    {
        public string Date { get; set; }

       // private DateTime dateFrom { get; set; }
       // private DateTime dateTo { get; set; }

        public string SearchTerm { get; set; }
        public string FieldTarget { get; set; }
        public int Count { get; set; }

    }
}