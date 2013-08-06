using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace NewsLibrarySearch
{
    public class NlAdvancedSearch
    {
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

        private int GetSearchResultCountFromHtml(string html)
        {
            int searchResultCount = 0;
            // Regular expression for finding search results phrase
            Regex r = new Regex("(<span class=\"basic-text-white\">)(Results: )([0-9]*)( - )([0-9]*)( of )([0-9]*)(</span>)");
            var matches = r.Matches(html);
            if (matches.Count > 0)
            {
                String src = matches[0].ToString();
                // Regular expression for finding total results in results phrase
                var match = Regex.Match(src, @"(?<=(\D|^))\d+(?=\D*$)");
                if (match.Success)
                {
                    searchResultCount = int.Parse(match.Value) + 1;
                }
            }
            return searchResultCount;
        }

        private bool NlRequest (out HttpWebResponse response, string date, string searchTerm, string fieldTarget)
        {
            response = null;

            try
            {
                HttpWebRequest request =
                    (HttpWebRequest) WebRequest.Create("http://nl.newsbank.com/nl-search/we/Archives");

                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:22.0) Gecko/20100101 Firefox/22.0";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.5");
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.Referer = "http://nl.newsbank.com/nl-search/we/Archives";
                request.KeepAlive = true;
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                NlSearchBody searchBody = new NlSearchBody(date, searchTerm, fieldTarget);
                string body = searchBody.Body;

                byte[] postBytes = System.Text.Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
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
    }
}
