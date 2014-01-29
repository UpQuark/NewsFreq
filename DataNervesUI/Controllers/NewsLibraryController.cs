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
        /// <returns>Query object's count</returns>
        public string PostCount(QueryModel queryModel)
        {
            var query = queryModel.Query;

            List<NlQuery> queries;
            DateTime dateFrom;
            DateTime dateTo;

            switch (queryModel.SearchType)
            {
                // Search every month from Month/StartDay to Month+1/StartDay-1
                case "Monthly":
                    dateFrom = new DateTime(query.DateFrom.Year, query.DateFrom.Month, query.DateFrom.Day);
                    dateTo = query.DateTo.AddMonths(1).AddDays(-1);

                    queries = new List<NlQuery>();

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
                    return new JavaScriptSerializer().Serialize(queries);
                    
                // Search every year from year/month/day to year+1/month/day-1
                case "Annual":
                    dateFrom = new DateTime(query.DateFrom.Year, query.DateFrom.Month, query.DateFrom.Day);
                    dateTo = query.DateTo.AddYears(1).AddDays(-1);
                    queries = new List<NlQuery>();

                     Parallel.For(0, YearsBetween(dateFrom, dateTo), i =>
                                                {
                                                    var yearQuery = new NlQuery
                                                                        {
                                                                            SearchString = query.SearchString,
                                                                            DateFrom = dateFrom.AddYears(i),
                                                                            DateTo = dateFrom.AddYears(i+1).AddDays(-1)
                                                                        };
                                                    yearQuery.SendQuery();
                                                    queries.Add(yearQuery);
                                                });
                    queries.Sort((a, b) => a.DateFrom.CompareTo(b.DateFrom));
                    return new JavaScriptSerializer().Serialize(queries);

                case "None":
                    query.SendQuery();
                    queries = new List<NlQuery>();
                    queries.Add(query);
                    return new JavaScriptSerializer().Serialize(queries);
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
                monthsBetween--;
            }
            return monthsBetween;
        }

        private int YearsBetween (DateTime startDate, DateTime endDate)
        {
            int yearsBetween = 0;
            if (startDate.Year != endDate.Year)
            {
                for (DateTime date = startDate; date < endDate; date = date.AddYears(1))
                {
                    yearsBetween++;
                }
                yearsBetween--;
            }
            return yearsBetween;
        }
    }
}