/*
 * NewsLibrary client-side request formation from UI and handling of existing data
 */

// Search results are stored in array for display and manipulation
var results = new Array();

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
                drawTable(results);
            } else {
                results.push($.parseJSON(data));
                //results = $.parseJSON(data);
                drawTable(results);
            }
        }
    });
}

// Draw table in DOM from results array
function drawTable(results) {
    var tblBody = "";
    $.each(results, function (r, v) {
        var tblRow = "";
        tblRow += "<td>" + v.SearchString + "</td>";
        tblRow += "<td>" + getDateString(v.DateFrom) + "</td>";
        tblRow += "<td>" + getDateString(v.DateTo) + "</td>";
        tblRow += "<td>" + v.Count.toString() + "</td>";
        tblBody += "<tr>" + tblRow + "</tr>";
    });
    $("#NewsDataTableContent").html(tblBody);
    $('#NewsDataTable').show(); //Table starts hidden when unpopulated
    drawChart();
}

// Converts JSON date notation to mm/dd/yy string
function getDateString(jsonDate) {
    var date = new Date(parseInt(jsonDate.substr(6)));
    return (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
}

// Draw chart from ALL data in table
function drawChart() {
    var resultsCount = new Array();
    $.each(results, function(r, v) {
        resultsCount.push(v.Count);
    });

    var resultsLabels = new Array();
    $.each(results, function(r, v) {
        resultsLabels.push(getDateString(v.DateFrom));
    });
        
    var lineChartData = {
        labels: resultsLabels,
        datasets: [
                    {
                        fillColor: "rgba(220,220,220,0.5)",
                        strokeColor: "rgba(150,150,220,1)",
                        pointColor: "rgba(120,220,220,1)",
                        pointStrokeColor: "#fff",
                        data: resultsCount
                    }
                ]
    };

    var lineChartOptions = {
        bezierCurve: false
    };

    var myLine = new Chart(document.getElementById("NewsDataChart").getContext("2d")).Line(lineChartData, lineChartOptions);
    $('#NewsDataChart').show(); //Chart starts hidden when unpopulated
}
