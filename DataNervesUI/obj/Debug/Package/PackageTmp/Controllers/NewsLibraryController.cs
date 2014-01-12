using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;
using DataNervesUI.Models;
using NewsLibrarySearch.API;

namespace DataNervesUI.Controllers
{
    public class NewsLibraryController : ApiController
    {
        /// <summary>
        /// Sends special searches to backend based on searchType
        /// </summary>
        /// <param name="queryModel">Query request data</param>
        /// <param name="searchType">String identifier of special search type</param>
        /// <returns>Query object's count</returns>
        public string PostCount(QueryModel queryModel)
        {
            var query = queryModel.Query;
            switch (queryModel.SearchType)
            {
                // Search every month from the 1st to last day of that month. Ignore day variables
                case "Monthly":
                    var dateFrom = new DateTime(query.DateFrom.Year, query.DateFrom.Month, 1);
                    var dateTo = query.DateTo.AddMonths(1).AddDays(-1);

                    var queries = new List<NlQuery>();

                   /* for (DateTime date = dateFrom; date < dateTo; date = date.AddMonths(1) )
                    {
                        var monthQuery = new NlQuery();
                        monthQuery.SearchString = query.SearchString;
                        monthQuery.DateFrom = date;
                        monthQuery.DateTo = date.AddMonths(1).AddDays(-1);
                        monthQuery.SendQuery();
                        queries.Add(monthQuery);
                    }*/

                    Parallel.For(0, MonthsBetween(dateFrom, dateTo), i =>
                                                                         {
                                                                             var monthQuery = new NlQuery
                                                                                                  {
                                                                                                      SearchString = query.SearchString,
                                                                                                      DateFrom = dateFrom.AddMonths(i),
                                                                                                      DateTo = dateFrom.AddMonths(i+1).AddDays(-1)
                                                                                                  };
                                                                             monthQuery.SendQuery();
                                                                             queries.Add(monthQuery);
                                                                         });
                    queries.Sort((a, b) => a.DateFrom.CompareTo(b.DateFrom));
                    var json = new JavaScriptSerializer().Serialize(queries);
                    return json;

                case "Yearly":
                    break;

                case "None":
                    query.SendQuery();
                    return new JavaScriptSerializer().Serialize(query);
            }
            return new JavaScriptSerializer().Serialize(query);
        }

        /// <summary>
        /// Calculates the number of months between two dates, inclusively with first and last
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        private int MonthsBetween(DateTime startDate, DateTime endDate)
        {
            int monthsBetween = 0;
            if (startDate.Month != endDate.Month || startDate.Year != endDate.Year)
            {
                for (DateTime date = startDate; date < endDate; date = date.AddMonths(1))
                {
                    monthsBetween++;
                }
            }
            return monthsBetween;
        }
    }
}