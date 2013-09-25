using System;
using System.Runtime.Serialization;

namespace CensusRetriever.QueryRetriever
{
    [Serializable]
    public class CensusQueryException : Exception
    {
        //Custom properties
        public string Url { get; private set; }
        public string ErrorMessage { get; private set; }


        public CensusQueryException()
        {
        }

        public CensusQueryException(string message) : base(message)
        {
        }

        public CensusQueryException(string message, string url, string errorMessage)
            : base(message)
        {
        }

        public CensusQueryException(string message, Exception inner) : base(message, inner)
        {
        }

        protected CensusQueryException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
