<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="NewsLibrary.aspx.cs" Inherits="NewsLibrarySearchUI.NewsLibrary" %>
<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="MainContent" runat="server">
    <script src="Scripts/jquery-1.4.1.min.js" type="text/javascript"></script>
    <script src="Scripts/NlQueries.js" type="text/javascript" ></script>
    <script src="http://code.jquery.com/ui/1.10.3/jquery-ui.js" type="text/javascript"></script>
      <script>
          $(function () {
              $(".datepicker").datepicker();
              $(".datepicker").datepicker("option", "showAnim", "drop");
          });
      </script>

    <p>You can use the operators AND, OR, and NOT to refine results, as well as quotation marks.</p>
    <p>Enter terms to search with or without boolean operators</p>
     
    <input type="text" id="searchText" class="SearchForm" placeholder="Enter search terms"/>
    <table>
        <tr>
            <td>Date from: </td><td><input type="text" id="dateFrom"class="datepicker" placeholder="Select from date"/></td>
        </tr>
        <tr>
            <td>Date to: </td><td><input type="text" id="dateTo"class="datepicker" placeholder="Select to date"/></td>
        </tr>
     </table>
     
     
     <input type=""/>
         <asp:DropDownList ID="FieldTargetsList" CssClass="dropDown" style="color: #5c5c5c;" runat="server">
        <asp:ListItem>All Text</asp:ListItem>
        <asp:ListItem>Lede</asp:ListItem>
        <asp:ListItem>Headline</asp:ListItem>
        <asp:ListItem>Index Terms</asp:ListItem>
        <asp:ListItem>Author</asp:ListItem>
        <asp:ListItem>Section</asp:ListItem>
        <asp:ListItem>Edition</asp:ListItem>
        <asp:ListItem>Caption</asp:ListItem>
        <asp:ListItem>Page</asp:ListItem>
        <asp:ListItem>Source</asp:ListItem>
    </asp:DropDownList>
    

    <input type = "button" id="newSearchButton" onclick="sendServerQuery()" value="New search"/>
    <input type = "button" id="clearSearchButton" value="Clear search"/>
    

    <div id="crap"></div>

</asp:Content>
