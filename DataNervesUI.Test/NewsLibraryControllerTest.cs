using DataNervesUI.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;

namespace DataNervesUI.Test
{
    
    
    /// <summary>
    ///This is a test class for NewsLibraryControllerTest and is intended
    ///to contain all NewsLibraryControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class NewsLibraryControllerTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for YearsBetween
        ///</summary>
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\sennis\\Documents\\NewsLibrary\\NewsLibrary\\DataNervesUI", "/")]
        [UrlToTest("http://localhost:52143/")]
        [DeploymentItem("DataNervesUI.dll")]
        public void YearsBetweenTest()
        {
            NewsLibraryController_Accessor target = new NewsLibraryController_Accessor(); // TODO: Initialize to an appropriate value
            DateTime startDate = DateTime.Parse("1/1/2001");
            DateTime endDate = DateTime.Parse("1/1/2002");
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.YearsBetween(startDate, endDate);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for MonthsBetween
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\sennis\\Documents\\NewsLibrary\\NewsLibrary\\DataNervesUI", "/")]
        [UrlToTest("http://localhost:52143/")]
        [DeploymentItem("DataNervesUI.dll")]
        public void MonthsBetweenTest()
        {
            NewsLibraryController_Accessor target = new NewsLibraryController_Accessor(); // TODO: Initialize to an appropriate value
            DateTime startDate = new DateTime(); // TODO: Initialize to an appropriate value
            DateTime endDate = new DateTime(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.MonthsBetween(startDate, endDate);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
