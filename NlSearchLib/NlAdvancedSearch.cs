using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace NewsLibrarySearch
{
    public class NlAdvancedSearch
    {

        #region constructors
        #endregion

        #region public methods

        /// <summary>
        /// Send newslibrary query with searchTerm only
        /// </summary>
        /// <param name="searchTerm">search criteria</param>
        /// <returns></returns>
        public int SendSearchRequest(string searchTerm)
        {
            HttpWebResponse response;
            int searchResultCount = 0;
            if (NlRequest(out response, "", searchTerm, ""))
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseHtml = reader.ReadToEnd();
                searchResultCount = GetSearchResultCountFromHtml(responseHtml);
                response.Close();
            }
            return searchResultCount;
        }

        /// <summary>
        ///  Send newslibrary query
        /// </summary>
        /// <param name="date">stringified date</param>
        /// <param name="searchTerm">search criteria</param>
        /// <param name="fieldTarget">field to search</param>
        public int SendSearchRequest(string date, string searchTerm, string fieldTarget)
        {
            HttpWebResponse response;
            int searchResultCount = 0;
            if (NlRequest(out response, date, searchTerm, fieldTarget))
            {
                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                string responseHtml = reader.ReadToEnd();
                searchResultCount = GetSearchResultCountFromHtml(responseHtml);
                response.Close();
            }
            return searchResultCount;
        }
        
        #endregion

        #region private methods

        private readonly Regex _regexResultsSpan = new Regex("(<span class=\"basic-text-white\">)(Results: )([0-9]*)( - )([0-9]*)( of )([0-9]*)(</span>)");
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

        /// <summary>
        /// Send HTTP POST request to NewsLibrary, return success/failure and response
        /// </summary>
        /// <param name="response"></param>
        /// <param name="date"></param>
        /// <param name="searchTerm"></param>
        /// <param name="fieldTarget"></param>
        /// <returns></returns>
        private bool NlRequest (out HttpWebResponse response, string date, string searchTerm, string fieldTarget)
        {
            string nlUri = "http://nl.newsbank.com/nl-search/we/Archives";

            response = null;
            try
            {
                //Build request data
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(nlUri);
                //request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:22.0) Gecko/20100101 Firefox/22.0";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.Referer = nlUri;
                request.KeepAlive = true;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                NlSearchBody searchBody = new NlSearchBody(date, searchTerm, fieldTarget);
                string body = searchBody.Body;
                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;

                //Send request
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

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
       
        #endregion
    }
}
