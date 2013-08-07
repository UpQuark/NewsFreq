using System;

namespace NewsLibrarySearch
{
    /// <summary>
    /// NewsLibrary search data. Both sent and recieved.
    /// </summary>
    public class NlSearchData
    {
        public string DateString { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime Time { get; set; }
        public string SearchTerm { get; set; }
        public string FieldTarget { get; set; }
        public int Count { get; set; }
    }
}