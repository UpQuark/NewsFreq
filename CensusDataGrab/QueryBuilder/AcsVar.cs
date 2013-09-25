using System;

namespace CensusRetriever.QueryBuilder
{
    public class AcsVar : CensusVar
    {
        public AcsVar(String w) : base(w)
        {
            String[] components = w.Split('_');
            Table = components[0];
            SubVar = components[1].Substring(0, 3);
            SubType = components[1].Substring(3, 1);

            // SubVar = w.Substring(7, 3);
            // SubType = w.Substring(10, 1);
            // Table = w.Substring(0, 6);
        }
    }
}