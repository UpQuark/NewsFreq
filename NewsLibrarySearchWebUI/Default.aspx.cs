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
            for (int i = DateTime.Now.Year; i >= 1978 ; i--)
            {
                DateRangeFromYear.Items.Add(i.ToString());
            }
            
            //Set gridView appearance characteristics
            resultsGV.DataSource = resultsList;
            resultsGV.GridLines = GridLines.None;
            resultsGV.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            resultsGV.HorizontalAlign = HorizontalAlign.Left;
            resultsGV.Width = 720;
            resultsGV.CellPadding = 4;
            resultsGV.BorderWidth = 0;
            resultsGV.DataBind();
        }

        protected void LoadDaysInMonth(object sender, EventArgs e)
        {
            int monthAsInt = Convert.ToInt32(DateRangeFromMonth.SelectedItem.Value);
            int yearAsInt = Convert.ToInt32(DateRangeFromYear.SelectedItem.Text);
            daysInMonth = DateTime.DaysInMonth(yearAsInt, monthAsInt);

            List<int> dayList = new List<int>();


            DateRangeFromDay.DataSource = dayList;

            for (int i = 1; i <= daysInMonth; i++ )
            {
                dayList.Add(i);
            }

            DateRangeFromDay.DataBind();
        }

        protected void AddSearchMethod(object sender, EventArgs e)
        {
            DateTime dateFrom;
            DateTime dateTo;

            // Do not send search if dates are blank
            if (DateRangeFrom.Text != "" && DateRangeTo.Text != "")
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
            }
        
            //Get searchTerm and fieldTarget
            var searchTerm = SearchForm.Text;
            var fieldTarget = NlSearchTargets.Target(FieldTargetsList.Text);

            //Create new task
            Task.Factory.StartNew(() => MakeRequest(dateFrom, dateTo, searchTerm, fieldTarget, DateTime.Now));
            UpdatePanel1.Visible = true;
        }

        protected void UpdateMethod(object sender, EventArgs e)
        {
            resultsGV.DataBind();
        }

        protected void ClearSearchMethod(object sender, EventArgs e)
        {
            //Clear resultsLabel and clear resultsList
            UpdatePanel1.Visible = false;
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