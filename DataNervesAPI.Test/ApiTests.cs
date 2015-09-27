using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DataNervesAPI.Controllers;
using DataNervesAPI.Models;
using NewsLibrarySearch.API;

namespace DataNervesAPI.Test
{
    [TestClass]
    public class ApiTests
    {
        [TestMethod]
        public void TestNewsFreqTestController()
        {
            var testController = new NewsFreqTestController();
            var queryList = new List<NlQuery>();
            queryList.Add(new NlQuery
            {
                DateFrom = DateTime.Parse("09/01/2001"),
                DateTo = DateTime.Parse("09/01/2002"),
                SearchString = "Boston",
                SearchTarget = "Lede",
                SearchSource = ""
            });

            var response = testController.PostCount(new QueryModel
            {
                SearchType = "Monthly",
                Queries = queryList
            });
        }

        [TestMethod]
        public void TestNewsFreqController()
        {
            var controller = new NewsFreqController();
            var queryList = new List<NlQuery>();
            queryList.Add(new NlQuery
            {
                DateFrom = DateTime.Parse("09/01/2001"),
                DateTo = DateTime.Parse("09/01/2002"),
                SearchString = "Boston",
                SearchTarget = "Lede",
                SearchSource = ""
            });

            var response = controller.PostCount(new QueryModel
            {
                SearchType = "Monthly",
                Queries = queryList
            });
        }
    }
}
