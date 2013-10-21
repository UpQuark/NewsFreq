using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace NewsLibrarySearch.API
{
    public class NlQuery
    {
        #region Constants

        private const string NlUriBase = "http://nl.newsbank.com/nl-search/we/Archives";

        private const string NlUriStem = "/?s_siteloc=NL2&p_queryname=4000&p_action=search&p_product=NewsLibrary&p_theme=newslibrary2&s_search_type=customized&d_sources=location&d_place=United%20States&p_nbid=&p_field_psudo-sort-0=psudo-sort&f_multi=&p_multi=&p_widesearch=smart&p_sort=YMD_date%3aD&p_maxdocs=200&p_perpage=1";

        #endregion

        #region Fields

        private int _count = -1;
        #endregion


        #region Properties

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public String DateString { get; set; }
        public String SearchString { get; set; }
        public String SearchTarget { get; set; }
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

        public DateTime CreatedDate { get; private set; }

        #endregion

        #region Constructors

        public NlQuery()
        {
            
        }

        public NlQuery(DateTime dateFrom, DateTime dateTo, String dateString, String searchString, String searchTarget)
        {
            DateFrom = dateFrom;
            DateTo = dateTo;
            DateString = dateString;
            SearchString = searchString;
            SearchTarget = searchTarget;
            Count = Send();
            CreatedDate = DateTime.Now;
        }


        #region Public Methods

        public void SendQuery()
        {
            Count = Send();
        }

        #endregion



        #endregion

        #region Protected Methods

        /// <summary>
        ///  Send newslibrary query
        /// </summary>
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

        private Uri CreateUri()
        {
            return new Uri
                (
                NlUriBase
                + NlUriStem
                + "&p_text_base-0="
                + SearchString
                + "&p_text_base-0="
                + SearchString
                + "&p_field_base-0="
                + "&p_bool_base-1=AND&p_text_base-1=&p_field_base-1=Section&p_bool_base-2=AND&p_text_base-2="
                + "&p_field_base-2="
                + DateString
                + "&p_field_YMD_date-0=YMD_date&p_params_YMD_date-0=date%3AB%2CE&p_field_YMD_date-3=YMD_date&p_params_YMD_date-3=date%3AB%2CE&Search.x=18&Search.y=18"
                );
        }
        #endregion

        #region private methods

        private readonly Regex _regexResultsSpan =
            new Regex("(<span class=\"basic-text-white\">)(Results: )([0-9]*)( - )([0-9]*)( of )([0-9]*)(</span>)");

        // Regex for finding total results in results phrase
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
       
