<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="NewsLibrarySearchUI._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <p>You can use the operators AND, OR, and NOT to refine results, as well as quotation marks.</p>

    <p>Enter terms to search with or without boolean operators</p>
    <asp:TextBox ID="SearchForm" CssClass="SearchForm" runat="server">Search Terms</asp:TextBox>

    <asp:DropDownList ID="FieldTargetsList" runat="server">
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
    

    <p>Enter a date range to search or choose from the dropdown menu. Defaults to all documents. </p>
    <asp:TextBox ID="DateRangeFrom" CssClass="field" runat="server">Start date</asp:TextBox>  &nbsp; to &nbsp; 
    <asp:TextBox ID="DateRangeTo" CssClass="field" runat="server">End date</asp:TextBox>
    
    <p>Add search terms and click update to view results</p>
    <asp:Button ID="newSearch" runat="server" Text="Add search" OnClick="AddSearchMethod"/>
    <asp:Button ID="addTerm" runat="server" Text="Clear search" OnClick="ClearSearchMethod"/>
    
    <asp:ScriptManager ID="ScriptManager" runat="server" />
    <asp:UpdatePanel ID="UpdatePanel1" CssClass="updatePanel" UpdateMode="Always" Visible="False" runat="server">
        <ContentTemplate>
            <fieldset>
                <asp:Label ID="resultsLabel" runat="server" Text=""></asp:Label>
                <asp:Timer ID="Timer1" Interval="1000" OnTick="UpdateMethod" runat="server">
                </asp:Timer>
                <asp:GridView CssClass="dataDisplay" ID="resultsGV" runat="server" 
                    AllowSorting="true" AutoGenerateColumns="false" OnSorting="OnSort">
                    <Columns>
                        <asp:BoundField HeaderText="Term" DataField="SearchTerm" SortExpression="SearchTerm" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"/>
                        <asp:BoundField HeaderText="Date" DataField="Date" SortExpression="Date" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"/>
                        <asp:BoundField HeaderText="Field" DataField="FieldTarget" SortExpression="FieldTarget" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"/>
                        <asp:BoundField HeaderText="Occurances" DataField="Count" SortExpression="Count" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"/>
                    </Columns>
                
                </asp:GridView>
            </fieldset>
        </ContentTemplate>
        
    </asp:UpdatePanel>
    
</asp:Content>

<asp:Content ID="rightColumnContent" runat="server" ContentPlaceHolderID="SidebarContent">
    
</asp:Content>