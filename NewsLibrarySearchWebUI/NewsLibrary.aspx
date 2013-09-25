<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NewsLibrary.aspx.cs" Inherits="NewsLibrarySearchUI.NewsLibrary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <script src="Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="Scripts/NlSearchScripts.js" type="text/javascript" ></script>
    <script src="http://code.jquery.com/ui/1.10.3/jquery-ui.js"></script>
      <script>
          $(function () {
              $("#datepicker").datepicker();
              $("#datepicker").datepicker("option", "showAnim", "drop");
          });
        </script>

    <p>You can use the operators AND, OR, and NOT to refine results, as well as quotation marks.</p>
    <p>Enter terms to search with or without boolean operators</p>
     
    <input type="text" id="searchText" class="SearchForm" placeholder="Enter search terms"/>
    <p>Date: <input type="text" id="datepicker" /></p>
    

    <input type = "button" id="newSearchButton" onclick="searchClick()" value="New search"/>
    <input type = "button" id="clearSearchButton" value="Clear search"/>
    
    <div id="crap"></div>

</asp:Content>
