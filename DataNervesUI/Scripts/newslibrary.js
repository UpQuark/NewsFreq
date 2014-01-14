/*
 * NewsLibrary client-side request formation from UI and handling of existing data
 */

// Search results are stored in array for display and manipulation
function Results() {
    this.data = new Array();
}

var searchTerms = new Array;

Results.prototype.addVariable = function(variable) {
    this.data.push(variable);
};

Results.prototype.clear = function() {
    this.data = [];
};

var Results1 = new Results();
var results = new Array();

var frozenVariable = false;

// Clear results and hide displays
function clearResults() {
    results = [];
    drawTable(results);
    $('#NewsDataChart').hide();
    $('#NewsDataTable').hide();
    
    $("#SearchButton").removeAttr("disabled", "enabled");
    $("#SearchButton").removeClass('disabledButton');

    $("#AddVariableButton").attr("disabled", "disabled");
    $("#AddVariableButton").addClass('disabledButton');

    $('#GraphLegend').empty();

    Results1.clear();
    frozenVariable = false;
}


// Send search to the API backend using UI criteria.
function newsSearch() {
    var specialSearchType = 'None'; // Defaults to normal searches

    // Check special search types
    if ($('#Monthly').is(':checked')) { 
        specialSearchType = "Monthly";
    }
    if ($('#Annual').is(':checked')) {
        specialSearchType = "Annual";
    }

    if(specialSearchType != 'None') {
        $("#SearchButton").attr("disabled", "disabled");
        $("#SearchButton").addClass("disabledButton");

        $("#AddVariableButton").removeAttr("disabled");
        $("#AddVariableButton").removeClass('disabledButton');
        
        frozenVariable = true;
    }
    

    // Create params
    var params = {
        DateFrom: $('#DateFrom').val(),
        DateTo: $('#DateTo').val(),
        DateString: $('#DateFrom').val() + ' to ' + $('#DateTo').val(),
        SearchString: $('#SearchTerms').val(),
        SearchTarget: ''
    };
    
    // Send query to API
    $.ajax({
        url: 'api/NewsLibrary',
        type: "POST",
        data: { query: params, searchType: specialSearchType },
        dataType: "json",
        success: function (data) {
            if (specialSearchType != 'None') {
                //$.merge(results, $.parseJSON(data));
                results = $.parseJSON(data);
                Results1.addVariable(results);
                drawTable(Results1);
            } else {

                results.push($.parseJSON(data));
                Results1.addVariable(results);
                //results = $.parseJSON(data);
                drawTable(Results1);
            }
        }
    });
}

// Add variable to already existing search
function addVariable() {
    
}

// Draw table in DOM from results array
function drawTable(results) {
    var tblBody = "";
    $.each(results.data, function (a, b) {
        $.each(b, function(r, v) {
            var tblRow = "";
            tblRow += "<td>" + v.SearchString + "</td>";
            tblRow += "<td>" + getDateString(v.DateFrom) + "</td>";
            tblRow += "<td>" + getDateString(v.DateTo) + "</td>";
            tblRow += "<td>" + v.Count.toString() + "</td>";
            tblBody += "<tr>" + tblRow + "</tr>";
        });
    });
    $("#NewsDataTableContent").html(tblBody);
    $('#NewsDataTable').show(); //Table starts hidden when unpopulated
    drawChart(results);
}

// Converts JSON date notation to mm/dd/yy string
function getDateString(jsonDate) {
    var date = new Date(parseInt(jsonDate.substr(6)));
    return (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
}

// Draw chart from ALL data in table
function drawChart(results) {
    var searchTerm;
    searchTerms = [];

    var resultsLabels = new Array();
    $.each(results.data[0], function(r, v) {
        resultsLabels.push(getDateString(v.DateFrom));
    });

    var data = new Array();
    $.each(results.data, function (a, b) {
        var resultsCount = new Array();
        $.each(b, function (r, v) {
            resultsCount.push(v.Count);
            searchTerm = v.SearchString;
        });

        data.push(
        {
            fillColor: "rgba(220,220,220,0.0)",
            strokeColor: randomColor(searchTerm), //"#878787",
            pointColor: "rgba(179,215,224,1)",
            pointStrokeColor: "#fff",
            data: resultsCount
        });
    });
        
    var lineChartData = {
        labels: resultsLabels,
        datasets: data
    };

    var lineChartOptions = {
        bezierCurve: false
        //pointDot: false,
    };

    var myLine = new Chart(document.getElementById("NewsDataChart").getContext("2d")).Line(lineChartData, lineChartOptions);
    drawLegend();
    $('#NewsDataChart').show(); //Chart starts hidden when unpopulated
}

function randomColor(keyword) {
    var randColor = '#' + (0x1000000 + (Math.random()) * 0xffffff).toString(16).substr(1, 6);
    searchTerms.push({
        keyword: keyword,
        color: randColor
    });
    return randColor;

}

function drawLegend() {
    $('#GraphLegend').empty();
    $.each(searchTerms, function(a, b) {
        $('#GraphLegend').append('<span style="background-color:' + b.color + '; border: 1px #ccc solid">&nbsp&nbsp&nbsp&nbsp</span>&nbsp' + b.keyword + '&nbsp');
    });
    
}