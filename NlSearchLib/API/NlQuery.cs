using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace NewsLibrarySearch.API
{
    /// <summary>
    /// News Library query class. Encapsulates both the send and response data of a query in a single wrapper
    /// </summary>
    public class NlQuery
    {
        #region Constants

        // Base archive search URL
        private const string NlUriBase = "http://nl.newsbank.com/nl-search/we/Archives";

        // Static variables
        private const string NlUriStem = "/?s_siteloc=NL2&p_queryname=4000&p_action=search&p_product=NewsLibrary&p_theme=newslibrary2&s_search_type=customized&d_sources=location&d_place=United+States&p_nbid=&p_field_psudo-sort-0=psudo-sort&f_multi=&p_multi=NewsLibraryAll&p_widesearch=smart&p_sort=YMD_date%3aD&p_maxdocs=200&p_perpage=1";

        #endregion

        #region Fields

        // Flags _count as uninitialized
        private int _count = -1;

        private string _dateString = "";

        #endregion

        #region Properties

        // Search criteria sent as query
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public String DateString { 
            get
            {
                if (_dateString == "")
                {
                    _dateString = DateFrom + " to " + DateTo;
                    return _dateString;
                }
                return _dateString;
            } 
            private set { _dateString = value; }

        }
        public String SearchString { get; set; }
        public String SearchTarget { get; set; }
        public String SearchSource { get; set; }

        // Count recieved from response
        public int Count
        {
            get
            {
                if (_count == -1)
                {
                    SendQuery();
                }
                return _count;
            }
            private set { _count = value; }
        }

        // Created Date metadata for sorting.
        public DateTime CreatedDate { get; private set; }

        #endregion

        #region Constructors

        // Empty constructor.
        public NlQuery()
        {
            CreatedDate = DateTime.Now;
        }

        // Search constructor, sends query from passed search criteria
        public NlQuery(DateTime dateFrom, DateTime dateTo, String searchString, String searchTarget, String searchSource)
        {
            DateFrom = dateFrom;
            DateTo = dateTo;
            SearchString = searchString;
            SearchTarget = searchTarget;
            SearchSource = searchSource;
            DateString = DateString;
            CreatedDate = DateTime.Now;
            Count = Send();
        }

        // Copy constructor
        public NlQuery(DateTime dateFrom, DateTime dateTo, String searchString, String searchTarget, String searchSource, int count)
        {
            DateFrom = dateFrom;
            DateTo = dateTo;
            SearchString = searchString;
            SearchTarget = searchTarget;
            SearchSource = searchSource;
            DateString = DateString;
            CreatedDate = DateTime.Now;
            Count = count;
        }

        // Copy constructor
        public NlQuery(NlQuery query)
        {
            DateFrom = query.DateFrom;
            DateTo = query.DateTo;
            DateString = DateString;
            SearchString = query.SearchString;
            SearchTarget = query.SearchTarget;
            SearchSource = query.SearchSource;
            Count = query.Count;
            CreatedDate = DateTime.Now;
        }

        #endregion

        #region Public Methods

        // Send search Data to retrieve count.
        public void SendQuery()
        {
            DateString = DateString; // Create date string
            Count = Send();
        }

        #endregion

        #region Protected Methods

        /// <summary>
        ///  Send newslibrary query to NewsLibrary frontend via http GET request
        /// </summary>
        /// <returns>Returns number of results found for the given search criteria</returns>
        protected int Send()
        {
            HttpWebResponse response;
            var searchResultCount = 0;
            if (SendSearchRequest(out response))
            {
                var dataStream = response.GetResponseStream();
                if (dataStream != null)
                {
                    var reader = new StreamReader(dataStream);
                    var responseHtml = reader.ReadToEnd();
                    searchResultCount = GetSearchResultCountFromHtml(responseHtml);
                }
                response.Close();
            }
            return searchResultCount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <returns></returns>
        protected bool SendSearchRequest(out HttpWebResponse response)
        {
            response = null;
            try
            {
                var request = (HttpWebRequest) WebRequest.Create(CreateUri());
                request.KeepAlive = true;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "GET";
                request.ServicePoint.Expect100Continue = false;
                response = (HttpWebResponse) request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse) e.Response;
                else return false;
            }
            catch (Exception)
            {
                if (response != null) response.Close();
                return false;
            }
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private Uri CreateUri()
        {
            return new Uri
                (
                    NlUriBase
                    + NlUriStem
                    + "&p_text_base-0=" + SearchString
                    + "&p_field_base-0=" + SearchTarget
                    + "&p_bool_base-1=" + "AND"
                    + "&p_text_base-1=" + SearchSource
                    + "&p_field_base-1=" + "Source"
                    + "&p_text_YMD_date-0=" + DateString.Replace(@"/", "%2F") 
                    + "&p_field_YMD_date-0=" + "YMD_date"
                    + "&p_params_YMD_date-0=" + "date%3AB%2C"
                    + "&p_field_YMD_date-3=" + "YMD_date"
                    + "&p_params_YMD_date-3=" + "date%3AB%2CE"
                    + "&Search.x=" + "18" 
                    + "&Search.y=" + "18"
                    + "&Search=" + "Search"
                );
        }

        #endregion

        #region Private methods

        // Regex for locating label displaying results count
        private readonly Regex _regexResultsSpan =
            new Regex("(<span class=\"basic-text-white\">)(Results: )([0-9]*)( - )([0-9]*)( of )([0-9]*)(</span>)");

        // Regex for locating total results in results label
        private readonly Regex _regexResultsMaxValue = new Regex(@"(?<=(\D|^))\d+(?=\D*$)");

        /// <summary>
        /// Parse HTML to return quantity of search results
        /// </summary>
        /// <param name="html">String of parsable HTML</param>
        /// <returns>Number of search results</returns>
        private int GetSearchResultCountFromHtml(string html)
        {
            int searchResultCount = 0;
            var matches = _regexResultsSpan.Matches(html);
            if (matches.Count > 0)
            {
                String src = matches[0].ToString();
                var match = Regex.Match(src, _regexResultsMaxValue.ToString());
                if (match.Success)
                {
                    searchResultCount = int.Parse(match.Value) + 1;
                }
            }
            return searchResultCount;
        }

        #endregion
    }
}
       
