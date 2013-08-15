using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using NewsLibrarySearch;
using NewsLibrarySearch.Query;


namespace NewsLibrarySearchUI
{
    public partial class _Default : System.Web.UI.Page
    {
        public List<NlSearchData> resultsList;
        private int daysInMonth = 31;
        bool _yearSet;

        protected void Page_Load(object sender, EventArgs e)
        {
            //Get session variables
            resultsList = HttpContext.Current.Session["List"] as List<NlSearchData>;
            if (null == resultsList)
            {
                resultsList = new List<NlSearchData>();
                HttpContext.Current.Session["List"] = resultsList;
            }

            if( HttpContext.Current.Session["yearSet"] == null)
            {
                HttpContext.Current.Session["yearSet"] = false;
            } else
            {
                _yearSet = (bool) HttpContext.Current.Session["_yearSet"];
            }

            //Fill year drop down
            if (!_yearSet)
            {
                for (int i = DateTime.Now.Year; i >= 1978; i--)
                {
                    DateRangeFromYear.Items.Add(i.ToString());
                    DateRangeToYear.Items.Add(i.ToString());
                }
                _yearSet = true;
                HttpContext.Current.Session["_yearSet"] = _yearSet;
            }

            LoadDaysInMonth(DateRangeFromDay, null);
            LoadDaysInMonth(DateRangeToDay, null);

            //Set gridView appearance characteristics
            resultsGV.DataSource = resultsList;
            resultsGV.GridLines = GridLines.None;
            resultsGV.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            resultsGV.HorizontalAlign = HorizontalAlign.Left;
            resultsGV.Width = 720;
            resultsGV.CellPadding = 4;
            resultsGV.BorderWidth = 0;
        }

        //Add a search term to the search collection. Called by add search button.
        protected void AddSearchMethod(object sender, EventArgs e)
        {
            //Get data data from UI
            DateTime dateFrom = new DateTime(Convert.ToInt32(DateRangeFromYear.SelectedItem.Text),
                                    Convert.ToInt32(DateRangeFromMonth.SelectedItem.Value),
                                    Convert.ToInt32(DateRangeFromDay.SelectedItem.Text));
            DateTime dateTo = new DateTime(Convert.ToInt32(DateRangeToYear.SelectedItem.Text),
                                    Convert.ToInt32(DateRangeToMonth.SelectedItem.Value),
                                    Convert.ToInt32(DateRangeToDay.SelectedItem.Text));

            //Get params from UI
            string searchTerm = SearchForm.Text;
            string fieldTarget = NlSearchTargets.Target(FieldTargetsList.Text);
            SearchTypes searchType = (SearchTypes) Convert.ToInt32(searchTypeMenu.SelectedItem.Value);

            if (searchType.Equals(SearchTypes.Basic))
            {
                Task.Factory.StartNew(() =>
                {
                    resultsList.Add(NlQuery.MakeRequest(dateFrom, dateTo, searchTerm, fieldTarget, DateTime.Now));
                    resultsList.Sort((x, y) =>
                        x.TimeofQuery.CompareTo(y.TimeofQuery));
                });
            }
            else
            {
                switch (searchType)
                {
                    case SearchTypes.EveryYear:
                        break;
                }
            }

            //Create new task
            
            UpdatePanel.Visible = true;
        }

        //Sort
        protected void ListSortMethod(object sender, EventArgs e)
        {
            string expression = ((System.Web.UI.WebControls.GridViewSortEventArgs) (e)).SortExpression;
            switch (expression)
            {
                case "SearchTerm":
                    resultsList.Sort((x, y) =>
                        String.Compare(x.SearchTerm, y.SearchTerm));
                    break;
                case "DateFrom":
                    resultsList.Sort((x, y) =>
                        x.DateFrom.CompareTo(y.DateFrom));
                    break;
                case "DateTo":
                    resultsList.Sort((x, y) =>
                        x.DateTo.CompareTo(y.DateTo));
                    break;
                case "FieldTarget":
                    resultsList.Sort((x, y) =>
                        String.Compare(x.FieldTarget, y.FieldTarget));
                    break;
                case "Count":
                    resultsList.Sort((y, x) =>
                        x.Count.CompareTo(y.Count));
                    break;
            }
        }

        //Update the contents of the data grid. Called by updatePanel tick.
        protected void UpdateMethod(object sender, EventArgs e)
        {
            resultsGV.DataBind();
        }

        //Clear resultslist and hide UpdatePanel
        protected void ClearSearchMethod(object sender, EventArgs e)
        {
            UpdatePanel.Visible = false;
            resultsList.Clear();
        }

        //Update date drop down lists to reflect the correct number of days in a given month. Called by month drop down.
        protected void LoadDaysInMonth(object sender, EventArgs e)
        {
            DropDownList listMonth = (DropDownList)sender;
            DropDownList listDay;
            int monthAsInt;
            int yearAsInt;

            if (listMonth.ID.Contains("DateRangeFrom"))
            {
                listDay = DateRangeFromDay;
                monthAsInt = Convert.ToInt32(DateRangeFromMonth.SelectedItem.Value);
                yearAsInt = Convert.ToInt32(DateRangeFromYear.SelectedItem.Text);
            }
            else if (listMonth.ID.Contains("DateRangeTo"))
            {
                listDay = DateRangeToDay;
                monthAsInt = Convert.ToInt32(DateRangeFromMonth.SelectedItem.Value);
                yearAsInt = Convert.ToInt32(DateRangeFromYear.SelectedItem.Text);
            }
            else
            {
                throw new WebException("Date dropdowns do not have expected ID");
            }

            daysInMonth = DateTime.DaysInMonth(yearAsInt, monthAsInt);
            List<int> dayList = new List<int>();
            listDay.DataSource = dayList;
            for (int i = 1; i <= daysInMonth; i++)
            {
                dayList.Add(i);
            }
            listDay.DataBind();
        }
    }

    internal enum SearchTypes
    {
        Basic = 0,
        EveryYear,
        EveryMonth,
        EveryWeek,
        TenYears,
        TwelveMonths
    }
}