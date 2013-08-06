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
        private NlAdvancedSearch _search;
        private static int _counter = 0;
        public SortedList<long, NlSearchResult> resultsList;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            // Get list for given session
            resultsList = HttpContext.Current.Session["List"] as SortedList<long, NlSearchResult>;
            if (null == resultsList)
            {
                resultsList = new SortedList<long, NlSearchResult>();
                HttpContext.Current.Session["List"] = resultsList;
            }

            //Set gridView appearance characteristics
            resultsGV.DataSource = resultsList.Values;
            resultsGV.GridLines = GridLines.None;
            resultsGV.HeaderStyle.HorizontalAlign = HorizontalAlign.Left;
            resultsGV.HorizontalAlign = HorizontalAlign.Left;
            resultsGV.Width = 720;
            resultsGV.CellPadding = 4;
            resultsGV.BorderWidth = 0;
        }

        protected void AddSearchMethod(object sender, EventArgs e)
        {
            // gets dates, 
            var date = "";

            // concatonate dates if they are not blank
            if (DateRangeFrom.Text != "" && DateRangeTo.Text != "")
            {
                DateTime dateFrom = DateTime.Parse(DateRangeFrom.Text);
                DateTime dateTo = DateTime.Parse(DateRangeTo.Text);
                date = dateFrom.ToString("MMMM dd, yyyy") + " to " + dateTo.ToString("MMMM dd, yyyy");
            }
        
            //Get searchTerm and fieldTarget
            var searchTerm = SearchForm.Text;
            var fieldTarget = NlSearchTargets.Target(FieldTargetsList.Text);

            //Create new task
            long now = DateTime.Now.Ticks;
            Task.Factory.StartNew(() => MakeRequest(date, searchTerm, fieldTarget, now));
            UpdatePanel1.Visible = true;
        }

        protected void UpdateMethod(object sender, EventArgs e)
        {
            //Update label with contents of resultsList
      //      resultsLabel.Text = "<table width=760px>";
      //      foreach (var v in resultsList.Values)
      //      {
      //          string fieldKey = NlSearchTargets.Key(v.FieldTarget);
      //          resultsLabel.Text += "<tr><td class=\"resultsTable\" width=240px>" + v.SearchTerm + "</td><td>" +v.Date + "</td><td>   in " + fieldKey.ToLower() + "</td><td>  " + v.Count + " occurances</td></tr>";
      //      }
      //      resultsLabel.Text += "</table>";

            
            resultsGV.DataBind();
            //UpdatePanel1.Update();
        }

        protected void ClearSearchMethod(object sender, EventArgs e)
        {
            //Clear resultsLabel and clear resultsList
            UpdatePanel1.Visible = false;
            resultsLabel.Text = "";
            resultsList.Clear();
        }

     //   protected void OnSort(object sender, EventArgs e)
     //   {
     //       resultsGV.Sort("Count", SortDirection.Descending);
     //   }


        protected void OnSort(object sender, GridViewSortEventArgs e)
        {
            DataTable dataTable = resultsList.Values as DataTable;
            if (dataTable != null)
            {
                DataView dataView = new DataView(dataTable);
                dataView.Sort= e.SortExpression +" " + e.SortDirection;

                resultsGV.DataSource = dataView;
                resultsGV.DataBind();
            }


        }

        protected void MakeRequest(string date, string searchTerm, string fieldTarget, long now)
        {
            //Create a new search object
            _search = new NlAdvancedSearch();

            //Increment counter for ordered placement. redundant with increment below?
            _counter++;

            //Create search result to populate label
            NlSearchResult s = new NlSearchResult
            {
                SearchTerm = searchTerm,
                Date = date,
                FieldTarget = fieldTarget,
                Count = _search.SendSearchRequest(date, searchTerm, fieldTarget)
            };

            resultsList.Add(now, s);
        }


    }
}