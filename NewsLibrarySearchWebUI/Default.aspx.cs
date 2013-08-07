using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using NewsLibrarySearch;


namespace NewsLibrarySearchUI
{
    public partial class _Default : System.Web.UI.Page
    {
        public List<NlSearchResult> resultsList;
        private int daysInMonth = 31;
        bool yearSet = false;

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get list for given session
            resultsList = HttpContext.Current.Session["List"] as List<NlSearchResult>;
            if (null == resultsList)
            {
                resultsList = new List<NlSearchResult>();
                HttpContext.Current.Session["List"] = resultsList;
            }

            //Fill year drop down
            if (!yearSet)
            {
                for (int i = DateTime.Now.Year; i >= 1978; i--)
                {
                    DateRangeFromYear.Items.Add(i.ToString());
                    DateRangeToYear.Items.Add(i.ToString());
                }
                yearSet = true;
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
            //resultsGV.DataBind();

        }

        protected void LoadDaysInMonth(object sender, EventArgs e)
        {
            DropDownList listMonth = (DropDownList) sender;
            DropDownList listDay;
            DropDownList listYear;
            int monthAsInt;
            int yearAsInt;

            if (listMonth.ID.Contains("DateRangeFrom"))
            {
                listDay = DateRangeFromDay;
                monthAsInt = Convert.ToInt32(DateRangeFromMonth.SelectedItem.Value);
                yearAsInt = Convert.ToInt32(DateRangeFromYear.SelectedItem.Text);
            }

            else //if (listMonth.ID.Contains("DateRangeTo"))
            {
                listDay = DateRangeToDay;
                monthAsInt = Convert.ToInt32(DateRangeFromMonth.SelectedItem.Value);
                yearAsInt = Convert.ToInt32(DateRangeFromYear.SelectedItem.Text);
            }


            daysInMonth = DateTime.DaysInMonth(yearAsInt, monthAsInt);

            List<int> dayList = new List<int>();

            listDay.DataSource = dayList;

            for (int i = 1; i <= daysInMonth; i++ )
            {
                dayList.Add(i);
            }

            listDay.DataBind();
            
        }

        protected void AddSearchMethod(object sender, EventArgs e)
        {
            DateTime dateFrom;
            DateTime dateTo;

            // Do not send search if dates are blank
            /*if (DateRangeFrom.Text != "" && DateRangeTo.Text != "")
            {
                try
                {
                    dateFrom = DateTime.Parse(DateRangeFrom.Text);
                    dateTo = DateTime.Parse(DateRangeTo.Text);
                }
                catch (FormatException)
                {
                    return;
                }
            } else
            {
                return;
            }*/

            dateFrom = new DateTime(Convert.ToInt32(DateRangeFromYear.SelectedItem.Text),
                                    Convert.ToInt32(DateRangeFromMonth.SelectedItem.Value),
                                    Convert.ToInt32(DateRangeFromDay.SelectedItem.Text));
            dateTo = new DateTime(Convert.ToInt32(DateRangeToYear.SelectedItem.Text),
                                    Convert.ToInt32(DateRangeToMonth.SelectedItem.Value),
                                    Convert.ToInt32(DateRangeToDay.SelectedItem.Text));

            //Get searchTerm and fieldTarget
            var searchTerm = SearchForm.Text;
            var fieldTarget = NlSearchTargets.Target(FieldTargetsList.Text);

            //Create new task
            Task.Factory.StartNew(() => MakeRequest(dateFrom, dateTo, searchTerm, fieldTarget, DateTime.Now));
            UpdatePanel.Visible = true;
        }

        protected void UpdateMethod(object sender, EventArgs e)
        {
            resultsGV.DataBind();
        }

        protected void ClearSearchMethod(object sender, EventArgs e)
        {
            //Clear resultsLabel and clear resultsList
            UpdatePanel.Visible = false;
            resultsLabel.Text = "";
            resultsList.Clear();
        }

        /*protected void OnSort(object sender, GridViewSortEventArgs e)
        {
            DataTable dataTable = resultsList as DataTable;
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort= e.SortExpression +" " + e.SortDirection;

                resultsGV.DataSource = dataView;
                resultsGV.DataBind();
            }
        }*/

        protected void MakeRequest(DateTime dateFrom, DateTime dateTo, string searchTerm, string fieldTarget, DateTime now)
        {
            //Create a new search object
            NlAdvancedSearch search = new NlAdvancedSearch();

            string dateString = dateFrom.ToString("MMMM dd, yyyy") + " to " + dateTo.ToString("MMMM dd, yyyy");

            //Create search result to populate label
            NlSearchResult s = new NlSearchResult
            {
                SearchTerm = searchTerm,
                DateString = dateString,
                FieldTarget = fieldTarget,
                Count = search.SendSearchRequest(dateString, searchTerm, fieldTarget),
                Time = now,
            };

            resultsList.Add(s);
        }
    }
}