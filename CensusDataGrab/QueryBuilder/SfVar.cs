using System;

namespace CensusRetriever.QueryBuilder
{
public class SfVar : CensusVar {

    public SfVar(String w): base(w)
    {
        if ("PCT".Equals(w.Substring(0,2)))
        { /* Check for length special case */
            Table = w.Substring(0, 6);
            SubVar = w.Substring(6, 4);
        } 
        else 
        {
            Table = w.Substring(0, 4);
            SubVar = w.Substring(4, 4);
        }
        SubType = ""; /* SF variables have no margin of error / estimate type */
        }
    }
}
