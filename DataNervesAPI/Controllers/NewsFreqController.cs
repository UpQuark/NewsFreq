using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Script.Serialization;
using DataNervesAPI.Models;
using NewsLibrarySearch.API;

namespace DataNervesAPI.Controllers
{
    public class NewsFreqController : ApiController
    {
        public string GetCount([FromUri]QueryModel queryModel)
        {
            //var query = queryModel.Query;
            var queries = queryModel.Queries;
            var returnQueries = new List<List<NlQuery>>();

            //List<NlQuery> subQueries;
            DateTime dateFrom;
            DateTime dateTo;

            if (queryModel.SearchType == "Monthly"){
                foreach (NlQuery query in queries){
                    // Search every month from Month/StartDay to Month+1/StartDay-1
                        dateFrom = new DateTime(query.DateFrom.Year, query.DateFrom.Month, query.DateFrom.Day);
                        dateTo = query.DateTo.AddMonths(1).AddDays(-1);

                        // One query for each one month date increment
                        var subQueries = new List<NlQuery>();

                        Parallel.For(0, MonthsBetween(dateFrom, dateTo), i =>
                        {
                            var monthQuery = new NlQuery
                            {
                                SearchString = query.SearchString,
                                SearchTarget = query.SearchTarget,
                                SearchSource = query.SearchSource,
                                DateFrom = dateFrom.AddMonths(i),
                                DateTo = dateFrom.AddMonths(i + 1).AddDays(-1)
                            };
                            monthQuery.SendQuery();
                            subQueries.Add(monthQuery);
                        });
                        subQueries.Sort((a, b) => a.DateFrom.CompareTo(b.DateFrom));
                        returnQueries.Add(subQueries);
                    // return new JavaScriptSerializer().Serialize(subQueries);
                }
            }
            else if (queryModel.SearchType == "Annual"){
                foreach (NlQuery query in queries){
                    dateFrom = new DateTime(query.DateFrom.Year, query.DateFrom.Month, query.DateFrom.Day);
                        dateTo = query.DateTo.AddYears(1).AddDays(-1);
                        var subQueries = new List<NlQuery>();

                        Parallel.For(0, YearsBetween(dateFrom, dateTo), i =>
                        {
                            var yearQuery = new NlQuery
                            {
                                SearchString = query.SearchString,
                                SearchTarget = query.SearchTarget,
                                SearchSource = query.SearchSource,
                                DateFrom = dateFrom.AddYears(i),
                                DateTo = dateFrom.AddYears(i + 1).AddDays(-1)
                            };
                            yearQuery.SendQuery();
                            subQueries.Add(yearQuery);
                        });
                        subQueries.Sort((a, b) => a.DateFrom.CompareTo(b.DateFrom));
                        returnQueries.Add(subQueries);
                        //return new JavaScriptSerializer().Serialize(queries);
                }
            }
            return new JavaScriptSerializer().Serialize(returnQueries);
        }

        public string PostCount(QueryModel queryModel)
        {
            //var query = queryModel.Query;
            var queries = queryModel.Queries;
            var returnQueries = new List<List<NlQuery>>();

            //List<NlQuery> subQueries;
            DateTime dateFrom;
            DateTime dateTo;

            if (queryModel.SearchType == "Monthly")
            {
                foreach (NlQuery query in queries)
                {
                    // Search every month from Month/StartDay to Month+1/StartDay-1
                    dateFrom = new DateTime(query.DateFrom.Year, query.DateFrom.Month, query.DateFrom.Day);
                    dateTo = query.DateTo.AddMonths(1).AddDays(-1);

                    // One query for each one month date increment
                    var subQueries = new List<NlQuery>();

                    Parallel.For(0, MonthsBetween(dateFrom, dateTo), i =>
                    {
                        var monthQuery = new NlQuery
                        {
                            SearchString = query.SearchString,
                            SearchTarget = query.SearchTarget,
                            SearchSource = query.SearchSource,
                            DateFrom = dateFrom.AddMonths(i),
                            DateTo = dateFrom.AddMonths(i + 1).AddDays(-1)
                        };
                        monthQuery.SendQuery();
                        subQueries.Add(monthQuery);
                    });
                    subQueries.Sort((a, b) => a.DateFrom.CompareTo(b.DateFrom));
                    returnQueries.Add(subQueries);
                    // return new JavaScriptSerializer().Serialize(subQueries);
                }
            }
            else if (queryModel.SearchType == "Annual")
            {
                foreach (NlQuery query in queries)
                {
                    dateFrom = new DateTime(query.DateFrom.Year, query.DateFrom.Month, query.DateFrom.Day);
                    dateTo = query.DateTo.AddYears(1).AddDays(-1);
                    var subQueries = new List<NlQuery>();

                    Parallel.For(0, YearsBetween(dateFrom, dateTo), i =>
                    {
                        var yearQuery = new NlQuery
                        {
                            SearchString = query.SearchString,
                            SearchTarget = query.SearchTarget,
                            SearchSource = query.SearchSource,
                            DateFrom = dateFrom.AddYears(i),
                            DateTo = dateFrom.AddYears(i + 1).AddDays(-1)
                        };
                        yearQuery.SendQuery();
                        subQueries.Add(yearQuery);
                    });
                    subQueries.Sort((a, b) => a.DateFrom.CompareTo(b.DateFrom));
                    returnQueries.Add(subQueries);
                    //return new JavaScriptSerializer().Serialize(queries);
                }
            }
            return new JavaScriptSerializer().Serialize(returnQueries);
        }

        #region Private helpers

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

        private int YearsBetween(DateTime startDate, DateTime endDate)
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

        #endregion

}