using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DataNervesAPI.Test
{
    [TestClass]
    public class ApiTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            using (var client = new HttpClient() {BaseAddress = new Uri("http://localhost.com:50819")} )
            {
                var response = client.GetAsync("/api/NewsLibrary").Result;

                if (response.IsSuccessStatusCode)
                {
                    // by calling .Result you are performing a synchronous call
                    var responseContent = response.Content;

                    // by calling .Result you are synchronously reading the result
                    string responseString = responseContent.ReadAsStringAsync().Result;

                    Console.WriteLine(responseString);
                }
            }
        }
    }
}
