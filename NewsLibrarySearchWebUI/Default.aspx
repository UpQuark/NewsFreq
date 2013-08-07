<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="NewsLibrarySearchUI._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <p>You can use the operators AND, OR, and NOT to refine results, as well as quotation marks.</p>
    
    <script language="javascript" type="text/javascript">
        clicked = false;
    </script>

    <p>Enter terms to search with or without boolean operators</p>
    <asp:TextBox ID="SearchForm" CssClass="SearchForm" runat="server" OnClick="if (clicked == false){this.value=''; clicked=true}">Search Terms</asp:TextBox>

    
    
    &nbsp; in &nbsp;
    <asp:DropDownList ID="FieldTargetsList" CssClass="dropDown" runat="server">
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
    <div id="DateDropDowns">    
    <asp:DropDownList ID="DateRangeFromMonth" OnSelectedIndexChanged="LoadDaysInMonth" AutoPostBack="true" CssClass="dropDown" runat="server">
        <asp:ListItem Value="1">January</asp:ListItem>
        <asp:ListItem Value="2">February</asp:ListItem>
        <asp:ListItem Value="3">March</asp:ListItem>
        <asp:ListItem Value="4">April</asp:ListItem>
        <asp:ListItem Value="5">May</asp:ListItem>
        <asp:ListItem Value="6">June</asp:ListItem>
        <asp:ListItem Value="7">July</asp:ListItem>
        <asp:ListItem Value="8">August</asp:ListItem>
        <asp:ListItem Value="9">September</asp:ListItem>
        <asp:ListItem Value="10">October</asp:ListItem>
        <asp:ListItem Value="11">November</asp:ListItem>
        <asp:ListItem Value="12">December</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList ID="DateRangeFromDay" CssClass="dropDown"  runat="server"></asp:DropDownList>
    <asp:DropDownList ID="DateRangeFromYear" CssClass="dropDown" runat="server"></asp:DropDownList>
    &nbsp; to &nbsp; 
    <asp:DropDownList ID="DateRangeToMonth" OnSelectedIndexChanged="LoadDaysInMonth" AutoPostBack="true" CssClass="dropDown" runat="server">
        <asp:ListItem Value="1">January</asp:ListItem>
        <asp:ListItem Value="2">February</asp:ListItem>
        <asp:ListItem Value="3">March</asp:ListItem>
        <asp:ListItem Value="4">April</asp:ListItem>
        <asp:ListItem Value="5">May</asp:ListItem>
        <asp:ListItem Value="6">June</asp:ListItem>
        <asp:ListItem Value="7">July</asp:ListItem>
        <asp:ListItem Value="8">August</asp:ListItem>
        <asp:ListItem Value="9">September</asp:ListItem>
        <asp:ListItem Value="10">October</asp:ListItem>
        <asp:ListItem Value="11">November</asp:ListItem>
        <asp:ListItem Value="12">December</asp:ListItem>
    </asp:DropDownList>
    <asp:DropDownList ID="DateRangeToDay" CssClass="dropDown" runat="server"></asp:DropDownList>
    <asp:DropDownList ID="DateRangeToYear" CssClass="dropDown" runat="server"></asp:DropDownList>

    </div>
    <div id="DateTextFields"></div>
    <%--
    <asp:RadioButtonList ID="RadioButtonList1" runat="server">
        <asp:ListItem>Button1</asp:ListItem>
        <asp:ListItem>Button2</asp:ListItem>
    </asp:RadioButtonList>
    --%>
    <p>Add search terms and click update to view results</p>
    <asp:Button ID="newSearch" runat="server" Text="Add search" OnClick="AddSearchMethod"/>
    <asp:Button ID="clearSearch" runat="server" Text="Clear search" OnClick="ClearSearchMethod"/>
    
    <asp:ScriptManager ID="ScriptManager" runat="server" />
    <asp:UpdatePanel ID="UpdatePanel" CssClass="updatePanel" AutoPostBack="true" UpdateMode="Always" Visible="False" runat="server">
        <ContentTemplate>
            <fieldset>
                <asp:Label ID="resultsLabel" runat="server" Text=""></asp:Label>
                <asp:Timer ID="Timer1" Interval="2000" OnTick="UpdateMethod" runat="server">
                </asp:Timer>
                <asp:GridView CssClass="dataDisplay" ID="resultsGV" runat="server" 
                    AllowSorting="true" OnSorting="ListSortMethod" AutoGenerateColumns="false">
                    <Columns>
                        <asp:BoundField HeaderText="Term" DataField="SearchTerm" SortExpression="SearchTerm" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"/>
                        <asp:BoundField HeaderText="Date" DataField="DateString" SortExpression="Date" ItemStyle-HorizontalAlign="Left" HeaderStyle-HorizontalAlign="Left"/>
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