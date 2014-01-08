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

    // Create params
    var params = {
        DateFrom: $('#DateFrom').val(),
        DateTo: $('#DateTo').val(),
        DateString: $('#DateFrom').val() + ' to ' + $('#DateTo').val(),
        SearchString: $('#SearchTerms').val(),
        SearchTarget: ''
    };

    $('#NewsDataTable').show();
    
    // Send query to API
    $.ajax({
        url: 'api/NewsLibrary',
        type: "POST",
        data: { query: params, searchType: specialSearchType },
        dataType: "json",
        success: function (data) {
            if (specialSearchType != 'None') {
                $.merge(results, $.parseJSON(data));
                drawTable(results);
            } else {
                results.push($.parseJSON(data));
                drawTable(results);
            }
        }
    });
}

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
}

function getDateString(jsonDate) {
    var date = new Date(parseInt(jsonDate.substr(6)));
    return (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
}