/*
 * NewsLibrary client-side request formation from UI and handling of existing data
 */

///////// Results data structure, for future expansion /////////
function Results() {
    this.data = new Array();
}

Results.prototype.addVariable = function(variable) {
    this.data.push(variable);
};

Results.prototype.clear = function () {
    this.data = [];
};
////////////////////////////////////////////////////////////////


var searchTerms = new Array;
var resultsData = new Results();
var colourValues = [
        "b3d7e0", "4564a5", "45a2a5", 
        "800000", "008000", "000080", "808000", "800080", "008080", "808080",
        "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0",
        "400000", "004000", "000040", "404000", "400040", "004040", "404040",
        "200000", "002000", "000020", "202000", "200020", "002020", "202020",
        "600000", "006000", "000060", "606000", "600060", "006060", "606060",
        "A00000", "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0",
        "E00000", "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0",
    ];

// Clear results and hide displays
function clearResults() {
    resultsData.clear();
    $('#NewsDataChart').hide();
    $('#NewsDataTable').hide();

    $("#SearchButton").prop('value', 'New search');
    $('input[name=specialSearches]').removeAttr('disabled');
    $('.DatePicker').removeAttr('disabled').removeClass('disabled');
    $('#GraphLegend').empty();
    searchTerms = [];

    //Reset color values to default
    colourValues = [
        "b3d7e0", "4564a5", "45a2a5", 
        "800000", "008000", "000080", "808000", "800080", "008080", "808080",
        "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0",
        "400000", "004000", "000040", "404000", "400040", "004040", "404040",
        "200000", "002000", "000020", "202000", "200020", "002020", "202020",
        "600000", "006000", "000060", "606000", "600060", "006060", "606060",
        "A00000", "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0",
        "E00000", "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0",
    ];
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

    // Alter UI for in-progress search
    if(specialSearchType != 'None') {
        $("#SearchButton").prop('value', 'Add variable');
        $('input[name=specialSearches]').attr('disabled', 'disabled');
        $('.DatePicker').prop('disabled', 'disabled').addClass('disabled');
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
                resultsData.addVariable($.parseJSON(data));
                drawTable(resultsData);
            } else {
                resultsData.addVariable($.parseJSON(data));
                drawTable(resultsData);
            }
        }
    });
}

// Draw table and chart in DOM from results array
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

// Draw chart from resultsData, and draw corresponding legend
function drawChart(results) {
    var searchTerm;
    //searchTerms = [];

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

        var color;
        var index = findWithAttr(searchTerms, 'keyword', searchTerm);
        if (index != -1) {
            color = searchTerms[index].color;
        } else {
            color = randomColor(searchTerm);
        }

        data.push(
        {
            fillColor: "rgba(220,220,220,0.0)",
            strokeColor: color,
            pointColor: color,
            pointStrokeColor: "#fff",
            data: resultsCount,
            title: searchTerm
        });
    });


    //Trim resultsLabels if they are too many
    var goalLength = 12;
    if (resultsLabels.length > goalLength) {
        var b = new Array();
        var n2 = resultsLabels.length - 2;
        var m2 = goalLength - 2;
        b[0] = resultsLabels[0];
        var j = 0;
        var k = 0;
        while (j < n2) {
            var diff = (k + 1) * n2 - (j + 1) * m2;
            if (diff < n2 / 2) {
                k += 1;
                j += 1;
                b[k] = resultsLabels[j];
            }
            else j += 1;
           }
        b[m2 + 1] = n2 + 1;

        for (var i = 0; i < resultsLabels.length; i++ ) {
            if (b.indexOf(resultsLabels[i]) == -1 ) {
                resultsLabels[i] = '';
            }
        }

            //resultsLabels = b;
    }

    var lineChartData = {
        labels: resultsLabels,
        datasets: data
    };

    var lineChartOptions = {
        bezierCurve: false,
        pointDot: false
    };

    var myLine = new Chart(document.getElementById("NewsDataChart").getContext("2d")).Line(lineChartData, lineChartOptions);
    drawLegend();
    $('#NewsDataChart').show(); //Chart starts hidden when unpopulated
}

// Draw key for graph in DOM
function drawLegend() {
    $('#GraphLegend').empty();
    $.each(searchTerms, function(a, b) {
        $('#GraphLegend').append('<span style="background-color: #' + b.color + ';">&nbsp&nbsp&nbsp&nbsp</span>&nbsp' + b.keyword.replace(/ /g, '&nbsp') + '&nbsp&nbsp ');
    });

}

//////////  Helper functions ////////// 
function randomColor(keyword) {
    //var index = Math.floor(Math.random() * colourValues.length);
    var color = colourValues[0];
    //if (index > -1) {
        colourValues.splice(0, 1);
    //}
    
    searchTerms.push({
        keyword: keyword,
        color: color
    });
    return color;
}

function findWithAttr(array, attr, value) {
    for (var i = 0; i < array.length; i += 1) {
        if (array[i][attr] === value) {
            return i;
        }
    }
    return -1;
}

// Converts JSON date notation to mm/dd/yy string
function getDateString(jsonDate) {
    var date = new Date(parseInt(jsonDate.substr(6)));
    return (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
}

