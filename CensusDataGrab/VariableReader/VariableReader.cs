using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace CensusRetriever.VariableReader
{
    public class VariableReader
    {
        private HttpWebRequest _req;

        public apivariables Vars { get; set; }

        public VariableReader()
        {
            
        }

        public void Deserialize()
        {
            Vars = null;
            string path = "http://www.census.gov/developers/data/acs_5yr_2011_var.xml";

            XmlSerializer serializer = new XmlSerializer(typeof (apivariables));

            _req = (HttpWebRequest) WebRequest.Create(path);
            _req.Method = "GET";
            HttpWebResponse resp = (HttpWebResponse) _req.GetResponse();
            if (resp.StatusCode == HttpStatusCode.OK)
            {
                using (StreamReader reader = new StreamReader(resp.GetResponseStream()))
                {
                    Vars = (apivariables) serializer.Deserialize(reader);
                    reader.Close();
                }
            }

            /*
            if (Vars != null)
            {
                foreach (var v in Vars.Items)
                {
                    foreach (var t in v.variable)
                    Console.WriteLine(t.name);
                }
            }
             */
        }

    }
}

