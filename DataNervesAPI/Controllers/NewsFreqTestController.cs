using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;
using DataNervesAPI.Models;
using NewsLibrarySearch.API;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Web;

namespace DataNervesAPI.Controllers
{
    public class NewsFreqTestController : ApiController
    {
        public string PostCount(QueryModel queryModel)
        {
            using (var client = new HttpClient())
            {
                var response = client.PostAsJsonAsync("http://www.newsfreq.com/DataNervesApi/api/NewsFreq", queryModel).Result;
                
                if (response.IsSuccessStatusCode)
                {
                    // by calling .Result you are performing a synchronous call
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    string responseString = responseContent.ReadAsStringAsync().Result;

                    //var serializer = new JavaScriptSerializer();
                    return responseString;.Replace(@"\", " ");
                }
            }
            return null;
        }
    }
}