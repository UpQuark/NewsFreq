using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace CensusRetriever.QueryRetriever
{
    public class QueryRetriever
    {
        //Properties
        public String[,] ResponseArray { get; private set; }
        public String    ResponseJson  { get; private set; }

        private readonly String _urlstring;
        private readonly HttpWebRequest _req;
        
        public QueryRetriever(Uri uri)
        {   
            _req = (HttpWebRequest) WebRequest.Create(uri);
            _urlstring = uri.ToString();
            try
            {
                ResponseArray = SendQuery();
            }
            catch (CensusQueryException e)
            {
                ResponseArray = null;
            }
        }
        public QueryRetriever(String uri)
        {
            _req = (HttpWebRequest) WebRequest.Create(uri);
            _urlstring = uri;
            try
            {
                ResponseArray = SendQuery();
            }
            catch(CensusQueryException e)
            {
                ResponseArray = null;
            }
        }

        private String[,] SendQuery()
        {
            _req.Method = "GET";
            HttpWebResponse resp = (HttpWebResponse) _req.GetResponse();
            
            switch (resp.StatusCode)
            {
                case HttpStatusCode.OK:
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        String data = reader.ReadToEnd();
                        ResponseJson = data;
                        return ParseJsonArray(data);
                    }
                case HttpStatusCode.NoContent:
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        throw new CensusQueryException("No content", _urlstring, reader.ReadToEnd());
                    }
                case HttpStatusCode.BadRequest:
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        throw new CensusQueryException("Bad request", _urlstring, reader.ReadToEnd());
                    }
                case HttpStatusCode.InternalServerError:
                    using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                    {
                        throw new CensusQueryException("Internal error", _urlstring, reader.ReadToEnd());
                    }
                default:
                    throw new CensusQueryException("Unknown response code");
            }
        }

        private String[,] ParseJsonArray(String json)
        {
            json = json.Replace("]", "");
            json = json.Replace("[", "");

            String[] lineArray = json.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
            int rows = lineArray.Length;
            int columns = lineArray[0].Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
            String[,] array = new string[columns,rows];

            for (int i = 0; i < rows; i++)
            {
                String[] line = Regex.Split(lineArray[i], "(?<=^[^\"]*(?:\"[^\"]*\"[^\"]*)*),(?=(?:[^\"]*\"[^\"]*\")*[^\"]*$)");
                Console.WriteLine(lineArray[i]);
                for (int j = 0; j < columns; j++)
                {
                    try
                    {
                        array[j, i] = line[j];
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        throw e;
                    }
                }
            }
            return array;
        }
    }
}
