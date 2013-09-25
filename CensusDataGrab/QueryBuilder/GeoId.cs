using System;
using System.Globalization;

namespace CensusRetriever.QueryBuilder
{
    //Stores both the type/name of a geographical ID and its actual value.
    //Note that you only require ONE GeoID instance per type of geography
    public class GeoId
    {
        public String GeographyType { get; private set; }
        public String GeographyId { get; private set; }

        //Add to a value
        public void AddVal(params int[] vals)
        {
            for (int i = 0; i < vals.Length ; i++)
            {
                this.GeographyId += "," + vals[i].ToString("G");
            }
        }

        public GeoId(String type, String id)
        {
            this.GeographyType = type;
            this.GeographyId = id;
        }

        public GeoId(String type, int id)
        {
            this.GeographyType = type;
            this.GeographyId = id.ToString("G");
        }
    }
}