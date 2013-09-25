/* QueryUrlBuilder class for U.S. Census Bureau API queries. */

using System;
using System.Collections.Generic;
using System.Text;

namespace CensusRetriever.QueryBuilder
{
    /*
     * Default query builder constructor
     * dataSet: Always designates the DATA SET queried (acs5, sf1, sf3)
     * year:    Designates year of query. Is not valid for all types
     *          - acs5 can be either 2010 or 2011 (representing last year of range)
     *          - sf1 and sf3 can be 1990, 2000 or 2010
     * type:    Designates the TYPE of query, from the lists :
     *          - http://www.census.gov/developers/data/
     *          - http://api.census.gov/data/2010/acs5/geo.html
     *          - http://api.census.gov/data/2011/acs5/geo.html
     * vars:    Variable IDs of data to be retrieved for given geographies
     * keys:    Census API key required to retrieve data
     * geoIDs:  designate geography IDs that correspond to query TYPE
     * displayNames: Parameters of index i in this array will have their
     *          names displayed if displayNames[i] == true
     */

    public struct Query
    {
        public String DataSet;
        public String Year;
        public String Type;
        public bool DisplayName;
        public String Key;
        public List<CensusVar> Vars;
        public List<GeoId> GeoIDs;
    }

    public class QueryUrlBuilder {

        //Url field
        private String _url;

        //Properties
        public String Url
        {
            get 
            { 
                _url = BuildUrl(DisplayName);
                return _url;
            }
            private set
            {
                _url = value;
            }
        }
        
        //Component Properties
        public String DataSet       { get; set; }
        public String Year          { get; set; }
        public String Type          { get; set; }
        public String Key           { get; set; }
        public List<CensusVar> Vars { get; set; }
        public List<GeoId> GeoIDs   { get; set; }
        public bool DisplayName     { get; set; }

        private const String Stem = "http://api.census.gov/data/";

        public QueryUrlBuilder(Query q)
        {
            this.DataSet = q.DataSet;
            this.Year = q.Year;
            this.Type = q.Type;
            this.DisplayName = q.DisplayName;
            this.Key = q.Key;
            this.Vars = q.Vars;
            this.GeoIDs = q.GeoIDs;
            _url = BuildUrl(DisplayName);
        }
        public QueryUrlBuilder(String dataSet, String year, String type, bool displayName, String key, List<CensusVar> vars, List<GeoId> geoIDs)
        {
            this.DataSet = dataSet;
            this.Year = year;
            this.Type = type;
            this.DisplayName = displayName;
            this.Key = key;
            this.Vars = vars;
            this.GeoIDs = geoIDs;
            _url = BuildUrl(DisplayName);
        }

        private String BuildUrl(Boolean displayName)
        {
            int idCount = GeoIDs.Count - 1;

            _url = Stem + Year + "/" + DataSet + "?" + "key=" + Key + "&get=";

            var outUrl = new StringBuilder(_url);

            for (var i = 0; i < Vars.Count; i++ )
            {
                outUrl.Append(Vars[i].Variable);
                if (i < Vars.Count - 1)
                {
                    outUrl.Append(",");
                }
            }
                if (displayName)
                {
                    outUrl.Append(",NAME");
                }
            outUrl.Append("&for=" + GeoIDs[idCount].GeographyType + ":" + GeoIDs[idCount].GeographyId + "&in=");
            for (int i = 0; i < idCount; i++)
            {
                var id = GeoIDs[i];
                outUrl.Append(id.GeographyType + ":");
                outUrl.Append(id.GeographyId);
                if (i < idCount - 1)
                {
                    outUrl.Append("+");
                }
            }
            return outUrl.ToString();
        }
    }
}