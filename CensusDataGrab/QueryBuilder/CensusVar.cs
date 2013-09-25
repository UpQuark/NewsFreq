using System;

namespace CensusRetriever.QueryBuilder
{

/*  ACS VARIABLE:  
 *        subvar
 *          |
 *  B00001_001E
 *     ^      ^
 *   table   subtype
 * 
 * SF VARIABLE:
 * 
 *  PCT006 0002
 *    ^      ^
 *  table   var
 */
    public class CensusVar
    {
        public String Table    { get; protected set; } // Table that var derives from
        public String SubVar   { get; protected set; } // Variable within table to retrieve
        public String SubType  { get; protected set; } // Variable subtype, e.g. estimate or MoE
        public String Variable { get; protected set; } // Total constructed variable

        /* Component-based constructor */
        public CensusVar(String table, String subvar, String type)
        {
            this.Table = table;
            this.SubVar = subvar;
            this.SubType = type;
            this.Variable = table + "_" + subvar + type;
        }

        /* Variable-based constructor, unsafe. Cannot resolve subfields in parent class without knowing query type*/
        public CensusVar(String w)
        {
            this.Table = null;
            this.SubVar = null;
            this.SubType = null;
            this.Variable = w;
        }
    }
}
