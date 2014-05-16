/*
 * NewsLibrary client-side request formation from UI and handling of existing data
 */

//QueryString jquery plugin. Put somewhere else
(function ($) {
    $.QueryString = (function (a) {
        if (a == "") return {};
        var b = {};
        for (var i = 0; i < a.length; ++i) {
            var p = a[i].split('=');
            if (p.length != 2) continue;
            b[p[0]] = decodeURIComponent(p[1].replace(/\+/g, " "));
        }
        return b;
    })(window.location.search.substr(1).split('&'))
})(jQuery);


/* Extensible results data structure */
//////////////////////////////////////////////////
function Results() {
    this.data = new Array();
}

Results.prototype.addVariable = function(variable) {
    this.data.push(variable);
};

Results.prototype.clear = function () {
    this.data = [];
};

Results.prototype.isEmpty = function() {
    if (this.data.length == 1 && this.data[0].length == 0) {
        return true;
    }
    return false;
};

var resultsData = new Results();
var weightData = new Results();

var searchKeywordColors = new Array();  // Store pairs of search terms with color to graph a
var ajaxRequests = new Array(); // Store abortable queries
var searchIncrement = 'None';
var searchWeighted = false;
var colourValues = [
        "b3d7e0", "4564a5", "45a2a5",
        "800000", "008000", "000080", "808000", "800080", "008080", "808080",
        "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0",
        "400000", "004000", "000040", "404000", "400040", "004040", "404040",
        "200000", "002000", "000020", "202000", "200020", "002020", "202020",
        "600000", "006000", "000060", "606000", "600060", "006060", "606060",
        "A00000", "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0",
        "E00000", "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0"
];




/* Publicly exposed methods */
//////////////////////////////////////////////////

function initializeUi() {
    // Initialize datepicker and tooltips
    $(".DatePicker").datepicker({ changeYear: true });
    $("span.question").hover(function () {
        $(this).append('<div class="tooltip"><p>Monthly searches retrieve one data point for each month in the date range. Annual searches retrieve one point per year. <strong>Single data points cannot be graphed.</strong></p></div>');
    }, function () {
        $("div.tooltip").remove();
    });
    
    // Set the body to 'loading' when an ajax request is in progress
    var $body = $("body");
    $(document).on({
        ajaxStart: function () {
            if (!$('#None').is(':checked')) {
                $body.addClass("loading");
            }
        },
        ajaxStop: function () {
            $body.removeClass("loading");
        }
    });
    
    // Add selection of source/parent to UI
    $('#SourceTypeRadioGroup > input[type=radio]').click(function() {
        if ($('#SourceRadio').is(':checked')) {
            $('#SearchSource').show();
            $('#ParentCompany').hide();
        }
        if ($('#ParentRadio').is(':checked')) {
            $('#ParentCompany').show();
            $('#SearchSource').hide();
        }
    });
}

// Get request handler
function getQueryString() {
    if ($.QueryString != null) {
        return {
            weight: Boolean($.QueryString['weight']),
            searchIncrement: $.QueryString['searchIncrement'],
            params: {
                DateFrom: $.QueryString['dateFrom'],
                DateTo: $.QueryString['dateTo'],
                DateString: $.QueryString['dateFrom'] + ' to ' + $.QueryString['dateTo'],
                SearchString: $.QueryString['searchString'],
                SearchTarget: $.QueryString['searchTarget'],
                SearchSource: $.QueryString['searchSource']
            }
        };
    }
}

// Execute a new search to backend
function newsSearch(queryStringExists) {
    // Kill function if validation fails
    if (!queryStringExists && !validateUserInput()) { 
        return;
    }

    var params;

    // Alter UI for in-progress search
    $("#SearchButton").prop('value', 'Add variable');
    $('input[name=searchIncrement]').attr('disabled', 'disabled');
    $('input[name=searchWeight]').attr('disabled', 'disabled');
    $('input[name=sourceType]').attr('disabled', 'disabled');
    $('.DatePicker').prop('disabled', 'disabled').addClass('disabled');
    $('#ClearButton').prop('disabled', '').removeClass('disabledButton');

    if (queryStringExists) {
        var queryStringData = getQueryString();
        params = queryStringData.params;
        searchWeighted = queryStringData.weight;
        searchIncrement = queryStringData.searchIncrement;
    } else {
        // Check search time increment
        if ($('#Monthly').is(':checked')) {
            searchIncrement = "Monthly";
        }
        if ($('#Annual').is(':checked')) {
            searchIncrement = "Annual";
        }

        // Check search display type
        if ($('#Weighted').is(':checked')) {
            searchWeighted = true;
        }
        if ($('#Unweighted').is(':checked')) {
            searchWeighted = false;
        }
        
        // Create params
        params = {
            DateFrom: $('#DateFrom').val(),
            DateTo: $('#DateTo').val(),
            DateString: $('#DateFrom').val() + ' to ' + $('#DateTo').val(),
            SearchString: $('#SearchTerms').val(),
            SearchTarget: $('#SearchTargets').val(),
            SearchSource: $('#SearchSource').val()
        };
    }

    // Create weighting request params
    if (searchWeighted) {
        var weightParams;
        if (queryStringExists) {
            weightParams = getQueryString();
            weightParams.SearchString = '';
        } else {
            weightParams = {
                DateFrom: $('#DateFrom').val(),
                DateTo: $('#DateTo').val(),
                DateString: $('#DateFrom').val() + ' to ' + $('#DateTo').val(),
                SearchString: '',
                SearchTarget: $('#SearchTargets').val(),
                SearchSource: $('#SearchSource').val()
            };
        }
    }

    // Send query to API
    var keywordCountRequest = $.ajax({
        url: 'api/NewsLibrary',
        type: "POST",
        data: { query: params, searchType: searchIncrement },
        dataType: "json",
        success: function (data) {
            if (searchWeighted) {
                var weightCounts = $.ajax({
                    url: 'api/NewsLibrary',
                    type: "POST",
                    data: { query: weightParams, searchType: searchIncrement },
                    dataType: "json",
                    success: function (weight) {
                        resultsData.addVariable($.parseJSON(data));
                        weightData.addVariable($.parseJSON(weight));
                        drawVisuals(resultsData, weightData);
                    }
                });
            } else {
                resultsData.addVariable($.parseJSON(data));
                drawVisuals(resultsData);
            }
        }
    });
    ajaxRequests.push(keywordCountRequest);
}

// Clear all search data and reset UI
function clearResults() {
    resultsData.clear();
    
    $('#ErrorLabel').text('');
    $('#NewsDataGraph').hide();
    $('#NewsDataTable').hide();
    $('#NewsDataGraphLegend').empty();
    $('#NewsDataGraphLabel').hide();
    
    $('#DateFrom').removeClass('invalid');
    $('#DateTo').removeClass('invalid');
    $('#SearchTerms').removeClass('invalid');

    $("#SearchButton").prop('value', 'Search');
    $('input[name=searchIncrement]').removeAttr('disabled');
    $('input[name=searchWeight]').removeAttr('disabled');
    $('input[name=sourceType]').removeAttr('disabled');
    $('.DatePicker').removeAttr('disabled').removeClass('disabled');
    $('#ClearButton').prop('disabled', 'disabled').addClass('disabledButton');
    
    searchKeywordColors = [];
    
    // Abort all requests in progress
    $.each(ajaxRequests, function (a, b) {
        b.abort();
    });

    // Reset color values to default literal
    colourValues = [
        "b3d7e0", "4564a5", "45a2a5", 
        "800000", "008000", "000080", "808000", "800080", "008080", "808080",
        "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0",
        "400000", "004000", "000040", "404000", "400040", "004040", "404040",
        "200000", "002000", "000020", "202000", "200020", "002020", "202020",
        "600000", "006000", "000060", "606000", "600060", "006060", "606060",
        "A00000", "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0",
        "E00000", "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0"
    ];
}


/*  Internal methods  */
//////////////////////////////////////////////////

// Validate user input from DOM
function validateUserInput() {
    var valid = true;

    var dateFrom = $('#DateFrom').val();
    var dateTo = $('#DateTo').val();

    if (dateFrom == ''
        || dateFrom == null
        || (isNaN(Date.parse(dateFrom)))) {

        $('#DateFrom').addClass('invalid');
        valid = false;
    } else { $('#DateFrom').removeClass('invalid'); }

    if (dateTo == ''
        || dateTo == null
        || (isNaN(Date.parse(dateTo)))) {
        $('#DateTo').addClass('invalid');
        valid = false;
    } else { $('#DateTo').removeClass('invalid'); }

    if ($('#SearchTerms').val() == '' || $('#SearchTerms').val() == null) {
        $('#SearchTerms').addClass('invalid');
        valid = false;
    } else { $('#SearchTerms').removeClass('invalid'); }

    if (valid) {
        return true;
    }
    return false;
}

// Wraps data visualization drawing. Easier to expand with data field expansion.
function drawVisuals(results, weight) {
    if (typeof weight === "undefined") {
        weight = null;
    }
    
    if (results.isEmpty()) {
        clearResults();
        return;
    }

    var weightedResults = new Results();

    if (weight != null) {
        $.each(results.data, function (i, item) {
            weightedResults.data[i] = new Array();
            $.each(item, function (k, element) {
                var weightedResult = $.extend(true, {}, element);
                weightedResult.Count = element.Count / weight.data[i][k].Count;
                weightedResult.Count = weightedResult.Count * 100;
                weightedResult.Count = Math.round(weightedResult.Count * 100) / 100;
                weightedResults.data[i][k] = weightedResult;
            });
        });
    } else {
        weightedResults = results;
    }

    drawTable(weightedResults);
    drawChart(weightedResults);
}

// Draw table from results array,
function drawTable(results, weight) {
    if (weight != null) {

    }
    var tblBody = '<tr class="newsTableHead"><td>Keyword</td><td>Source</td><td>From</td><td>To</td><td>Instances</td></tr>';
    $.each(results.data, function (a, b) {
        $.each(b, function(r, v) {
            var tblRow = "";
            tblRow += "<td>" + v.SearchString + "</td>";
            tblRow += v.SearchSource !== null ?  "<td>" + v.SearchSource + "</td>" : "<td></td>";
            tblRow += "<td>" + getDateString(v.DateFrom) + "</td>";
            tblRow += "<td>" + getDateString(v.DateTo) + "</td>";
            tblRow += "<td>" + v.Count.toString() + "</td>";
            tblBody += "<tr>" + tblRow + "</tr>";
        });
    });
    $("#NewsDataTableContent").html(tblBody);
    $('#NewsDataTable').show(); // Table starts hidden when unpopulated
}

// Draw chart from resultsData, draw corresponding legend
function drawChart(results, weight) {
    if (results.data.length == 0) {
        clearResults();
        return;
    }

    if (weight != null) {

    }

    var searchString;
    var searchSource;

    // Drop all dateFrom strings into results labels array
    var chartLabels = new Array();
    $.each(results.data[0], function(r, v) {
        chartLabels.push(getDateString(v.DateFrom));
    });

    var chartQuantData = new Array();

    // Process all the results data into the data array
    $.each(results.data, function (a, b) {
        var resultsCount = new Array();
        $.each(b, function (r, v) {
            resultsCount.push(v.Count);
            searchString = v.SearchString;
            searchSource = v.SearchSource;
        });

        //Check if keyword already exists in searchKeywordColors and is already associated with a color, else make a new one
        var index = findElemByKeywordSource(searchKeywordColors, searchString, searchSource);
        if (index != -1) {
            var color = searchKeywordColors[index].color;
        } else {
            var color = getColor(searchString, searchSource);
        }

        // Add point data to data set
        chartQuantData.push(
        {
            fillColor: "rgba(220,220,220,0.0)",
            strokeColor: color,
            pointColor: color,
            pointStrokeColor: "#fff",
            data: resultsCount,
            title: searchString
        });
    });

    // Trim chartLabels to goalLength if length exceeds it
    var goalLength = 36;
    chartLabels = trimArray(chartLabels, goalLength);

    var lineChartData = {
        labels: chartLabels,
        datasets: chartQuantData
    };

    var lineChartOptions;


    //TODO: condense these into single block with amendment
    if (searchWeighted == false) {
        lineChartOptions = {
            bezierCurve: true,
            pointDot: false,
            scaleFontSize: 10,
        };
    }

    if (searchWeighted == true) {
        lineChartOptions = {
            bezierCurve: true,
            pointDot: false,
            scaleFontSize: 10,
            scaleLabel: "<%=value+'%'%>",
        };
    }

    

    // Create chart on canvas
    new Chart(document.getElementById("NewsDataGraph").getContext("2d")).Line(lineChartData, lineChartOptions);
    drawLegend(results, weight);
    $('#NewsDataGraph').show(); //Chart starts hidden when unpopulated
    var labelText = 'Articles Per {1} Featuring Keyword';
    if (searchIncrement == 'Monthly') {
        labelText = labelText.replace('{1}', 'Month');
    }
    else if (searchIncrement == 'Annual') {
        labelText = labelText.replace('{1}', 'Year');
    }
    $('#NewsDataGraphLabel').text(labelText).show();
}

// Draw key for graph in DOM
function drawLegend(results, weight) {
    $('#NewsDataGraphLegend').empty();
    $.each(searchKeywordColors, function (a, b) {
        var source = "";
        var sourcePrefix = "";
        if (b.source != null) {
            source = b.source;
            sourcePrefix = ":&nbsp";
        }
        $('#NewsDataGraphLegend').append(
            '<span class="NewsDataGraphLegendCell" data-source="'
            + source.replace(/ /g, '&nbsp')
            + '" data-keyword="'
            + b.keyword
            + '"><span style="background-color: #'
            + b.color
            + ';">&nbsp&nbsp&nbsp&nbsp&nbsp</span>&nbsp'
            + b.keyword.replace(/ /g, '&nbsp')
            + sourcePrefix
            + source
            + '&nbsp&nbsp </span>'
        );
        $('.NewsDataGraphLegendCell').click(function () { removeVariable($(this).data('keyword')); });
    });
    
    function removeVariable(keyword) {
        searchKeywordColors.splice(findWithAttr(searchKeywordColors, 'keyword', keyword), 1);
        $.each(results.data, function(a, b) {
            while (findWithAttr(b, 'SearchString', keyword) != -1) {
                b.splice(findWithAttr(b, 'SearchString', keyword), 1);
            }
        });
        drawVisuals(results, weight);
    }
}


/* Helper functions */
//////////////////////////////////////////////////
//Retrieve a color from the color array and remove that index
function getColor(keyword, source) {
    var color = colourValues[0];
    colourValues.splice(0, 1);
    searchKeywordColors.push({
        keyword: keyword,
        color: color,
        source: source
    });
    return color;
}

// Find index with attribute in array
function findWithAttr(array, attr, value) {
    for (var i = 0; i < array.length; i += 1) {
        if (array[i][attr] === value) {
            return i;
        }
    }
    return -1;
}

function findElemByKeywordSource(results, keyword, source) {
    for (var i = 0; i < results.length; i += 1) {
        if (results[i]['keyword'] === keyword && results[i]['source'] === source) {
            return i;
        }
    }
    return -1;
}

// Convert JSON date notation to mm/dd/yy string
function getDateString(jsonDate) {
    var date = new Date(parseInt(jsonDate.substr(6)));
    return (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
}

// Replaces elements of array with empty string at regular intervals until # of nonempty cells == goalLength
function trimArray(array, goalLength) {
    if (array.length > goalLength) {
        var b = new Array();
        var n2 = array.length - 2;
        var m2 = goalLength - 2;
        b[0] = array[0];
        var j = 0;
        var k = 0;
        while (j < n2) {
            var diff = (k + 1) * n2 - (j + 1) * m2;
            if (diff < n2 / 2) {
                k += 1;
                j += 1;
                b[k] = array[j];
            }
            else j += 1;
        }
        b[m2 + 1] = n2 + 1;

        for (var i = 0; i < array.length; i++) {
            if (b.indexOf(array[i]) == -1) {
                array[i] = '';
            }
        }
    }
    return array;
}
