////////////// NewsFreq class //////////////

// Constructor
function NewsFreq() {
    this.newsFreqSearchData = {

        keywordCounts: new this.Results(),   // Array of articles-including-keyword count by date range
        totalCounts: new this.Results(),   // Array of total article counts by date
        weightedKeywordCounts: new this.Results(), // Array of articles-including-keyword counts as a proportion of total article counts by date range
        
        queryString: null,

        // Settings storage for search time increment and weighted flag
        searchSettings: {
            searchIncrement: 'None',
            searchWeighted: false,
        },
        
        // Stores pairs of keywords and graph colors for consistency across searches
        searchKeywordColors: [],
        
        // Stores references to running ajax requests for cancelling
        ajaxRequests: [],

        searchHistory: [],
        
        // Color list from which keywords are matched
        colorValues: [               
            "b3d7e0", "4564a5", "45a2a5", "b00000", "00b000", "0000b0", "b0b000",
            "800000", "008000", "000080", "808000", "800080", "008080", "808080",
            "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0"
        ],
    };

    // Data structures encapsulating behavior of three main page elements
    this.table = new this.Table(this, this.newsFreqSearchData);
    this.graph = new this.Graph(this, this.newsFreqSearchData, this.table);
    this.form = new this.Form(this, this.newsFreqSearchData, this.table, this.graph);

    var newsFreq = this;

    //If URL contains a query string, store flag as true
    $(document).ready(function () { 
        if (window.location.search.substr(1) !== "") {
            newsFreq.newsFreqSearchData.queryString = $.deparam(window.location.search.substr(1));
            newsFreq.form.search();
        }
    });
}

// Resets all visual elements to default state, empties all data structures
NewsFreq.prototype.clear = function () {
    // Empty all data structures
    this.newsFreqSearchData.keywordCounts.clear();
    this.newsFreqSearchData.totalCounts.clear();
    this.newsFreqSearchData.weightedKeywordCounts.clear();

    this.newsFreqSearchData.searchKeywordColors = [];

    // Abort all requests in progress
    var ajaxRequests = this.newsFreqSearchData.ajaxRequests;
    if (ajaxRequests) {
        $.each(ajaxRequests, function (a, b) {
            b.abort();
        });
    }

    // Reset color values to default literal
    this.newsFreqSearchData.colorValues = [
        "b3d7e0", "4564a5", "45a2a5", "b00000", "00b000", "0000b0", "b0b000",
        "800000", "008000", "000080", "808000", "800080", "008080", "808080",
        "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0"
    ];

    // Reset all page elements to default state
    this.form.clear();
    this.graph.clear();
    this.table.clear();
    
};

////////////// Results class //////////////

NewsFreq.prototype.Results = function() {
     this.data = [];
};

// Add an additional variable array of data to results
NewsFreq.prototype.Results.prototype.addVariable = function(variable) {
     this.data.push(variable);
};

// Erase all contents
NewsFreq.prototype.Results.prototype.clear = function() {
     this.data = [];
};

// Is empty TODO: potentially not the right logic
NewsFreq.prototype.Results.prototype.isEmpty = function () {
    if (this.data.length == 1 && this.data[0].length == 0) {
        return true;
    }
    return false;
};


////////////// Form class //////////////
// Constructor
NewsFreq.prototype.Form = function (newsFreq, searchData, table, graph) {
    this.searchData = searchData;
    this.table = table;
    this.graph = graph;
    this.status = "enabled";

    $(document).keypress(function (e) {
        if (e.which === 13) {
            // enter pressed
            newsFreq.form.search();
        }
    });

    // Assign UI event handlers
    $('#SearchButton').click(function () {
        newsFreq.form.search();
    });
    $('#ClearButton').click(function () {
        newsFreq.clear();
    });

    // Initialize datepickers
    $(".DatePicker").datepicker({
        changeYear: true,
        constrainInput: true,
        yearRange: "1976:nn"
    });
    $("#DateFrom").datepicker("option", "defaultDate", "-1y");
    
    // Initialize helper tooltips deriving text from data element
    $("span.question").hover(function () {
        var tooltipText = $(this).data("tooltiptext");
        $(this).append('<div class="tooltip"><p>' + tooltipText + '</strong></p></div>');
    }, function () {
        $("div.tooltip").remove();
    });

    // Add ajax handlers to show loading wheel when request in progress
    $(document).on({
        ajaxStart: function () {
            if (!$('#None').is(':checked')) {
                $('.loadingIcon').show();
            }
        },
        ajaxStop: function () {
            $('.loadingIcon').hide();
        }
    });

    // Add selection of source/parent to UI
    $('#SourceTypeRadioGroup > input[type=radio]').click(function () {
        if ($('#SourceRadio').is(':checked')) {
            $('#SearchSource').show();
            $('#ParentCompany').hide();
        }
        if ($('#ParentRadio').is(':checked')) {
            $('#ParentCompany').show();
            $('#SearchSource').hide();
        }
    });
};

/* UI display and utility */
// Show form element
NewsFreq.prototype.Form.prototype.show = function () {
    $("#NewsFreqFormWrapper").show();
};

// Hide form element
NewsFreq.prototype.Form.prototype.hide = function () {
    $("#NewsFreqFormWrapper").hide();
};

// Enable form fields, erase all grey-outs and validation failures
NewsFreq.prototype.Form.prototype.enable = function () {
    $("#SearchButton").prop('value', 'Search');
    $('input[name=searchIncrement]').removeAttr('disabled');
    $('input[name=searchWeight]').removeAttr('disabled');
    $('input[name=sourceType]').removeAttr('disabled');
    $('.DatePicker').removeAttr('disabled').removeClass('disabled');
    $('#ClearButton').prop('disabled', 'disabled').addClass('disabledButton');
};

// Disable all form fields that should be inactive during active searches
NewsFreq.prototype.Form.prototype.disable = function () {
    $("#SearchButton").prop('value', 'Add variable');
    $('input[name=searchIncrement]').attr('disabled', 'disabled');
    $('input[name=searchWeight]').attr('disabled', 'disabled');
    $('input[name=sourceType]').attr('disabled', 'disabled');
    $('.DatePicker').prop('disabled', 'disabled').addClass('disabled');
    $('#ClearButton').prop('disabled', '').removeClass('disabledButton');
};

/* Getters */
// Get parameters from form field entries
NewsFreq.prototype.Form.prototype.getParams = function (weighted, queryString) {
    var params = [];
    var searchHistory = this.searchData.searchHistory;

    if (queryString !== null) {
        var queriesLength = queryString.Queries.length;

        //Set UI to contain values from QueryString
        $('#DateFrom').val(queryString.Queries[queriesLength - 1].DateFrom);
        $('#DateTo').val(queryString.Queries[queriesLength - 1].DateTo);
        $('#SearchTerms').val(queryString.Queries[queriesLength - 1].SearchString);
        $('#SearchTargets').val(queryString.Queries[queriesLength - 1].SearchTargets);
        $('#SearchSource').val(queryString.Queries[queriesLength - 1].SearchSource);

        params.push(queryString.Queries);
        $.merge(searchHistory, params);
        return queryString.Queries;
    }
    
    //if (searchHistory.length > 0) {
    //    $.each(searchHistory, function (key, keyword) {
    //        params.push({
    //            DateFrom: keyword.DateFrom,
    //            DateTo: keyword.DateTo,
    //            DateString: keyword.DateString,
    //            SearchString: keyword.SearchString,
    //            SearchTarget: keyword.SearchTarget,
    //            SearchSource: keyword.SearchSource
    //        });
    //    });
    //}

    // Set searchString to null on requests that are for total articles for time period
    var searchString = weighted ? "" : $('#SearchTerms').val();
    params.push({
        DateFrom: $('#DateFrom').val(),
        DateTo: $('#DateTo').val(),
        DateString: $('#DateFrom').val() + ' to ' + $('#DateTo').val(),
        SearchString: searchString,
        SearchTarget: $('#SearchTargets').val(),
        SearchSource: $('#SearchSource').val()
    });

    $.merge(searchHistory, params);
    return params;
};

// Get search settings from form field entries
NewsFreq.prototype.Form.prototype.getSearchSettings = function (queryString) {
    var searchSettings = {
        searchIncrement: '',
        searchWeighted: ''
    };
    
    // Check search time increment
    if (queryString) {
        searchSettings.searchIncrement = queryString.SearchType;
        searchSettings.searchWeighted = queryString.Queries["searchWeighted"] == "true" ? true : false;
        return searchSettings
    }

    if ($('#Monthly').is(':checked')) {
        searchSettings.searchIncrement = "Monthly";
    }
    if ($('#Annual').is(':checked')) {
        searchSettings.searchIncrement = "Annual";
    }
    // Check search display type
    if ($('#Weighted').is(':checked')) {
        searchSettings.searchWeighted = true;
    }
    if ($('#Unweighted').is(':checked')) {
        searchSettings.searchWeighted = false;
    }

    return searchSettings;
};


/* Data handling */
// Validate form field inputs
NewsFreq.prototype.Form.prototype.validateUserInput = function () {
    var valid = true;

    var dateFrom = $('#DateFrom').val();
    var dateTo = $('#DateTo').val();

    if (dateFrom == '' || dateFrom == null || (isNaN(Date.parse(dateFrom)))) {
        $('#DateFrom').addClass('invalid');
        valid = false;
    } else {
        $('#DateFrom').removeClass('invalid');
    }

    if (dateTo == '' || dateTo == null || (isNaN(Date.parse(dateTo)))) {
        $('#DateTo').addClass('invalid');
        valid = false;
    } else {
        $('#DateTo').removeClass('invalid');
    }

    if ($('#SearchTerms').val() == '' || $('#SearchTerms').val() == null) {
        $('#SearchTerms').addClass('invalid');
        valid = false;
    } else {
        $('#SearchTerms').removeClass('invalid');
    }

    if (valid) {
        return true;
    }
    return false;
};

// Fire search
NewsFreq.prototype.Form.prototype.search = function () {
    // Kill function if form validation fails and there is no queryString
    if (this.searchData.queryString == null && !this.validateUserInput()) {
        return;
    }
    // Disable UI for in progress search
    this.disable();

    var queryString = this.searchData.queryString;
    
    var searchWeighted = this.searchData.searchSettings.searchWeighted;
    this.searchData.searchSettings = this.getSearchSettings(queryString);
    var searchIncrement = this.searchData.searchSettings.searchIncrement;
    
    var searchHistory = this.searchData.searchHistory;
    var keywordCounts = this.searchData.keywordCounts;
    var totalCounts = this.searchData.totalCounts;
    var weightedKeywordCounts = this.searchData.weightedKeywordCounts;
    var table = this.table;
    var graph = this.graph;
    var ajaxRequests = this.searchData.ajaxRequests;

    // TODO: Do these need != null part?
    // Create request params
    var params = this.getParams(false, queryString);

    if (searchWeighted)
        var weightParams = this.getParams(true, queryString);

    var stateString = '?' + $.param({ "Queries": searchHistory, "SearchType": searchIncrement });
    window.history.replaceState(null, '', stateString);

    // Send query to API
    var keywordCountRequest = $.ajax({
        url: '/DataNervesApi/api/NewsFreq',
        type: "POST",
        dataType: "json",
        data: {
            Queries: params,
            SearchType: searchIncrement
        },
        success: function (data) {
            //var responseArray = $.parseJSON(data);
            $.merge(keywordCounts.data, $.parseJSON(data));
        }
    });
    ajaxRequests.push(keywordCountRequest);

    if (searchWeighted){
        var totalCountRequest = $.ajax({
            url: '/DataNervesApi/api/NewsFreq',
            type: "POST",
            dataType: "json",
            data: {
                Queries: weightParams,
                SearchType: searchIncrement
            },
            success: function (data) {
                $.merge(totalCounts.data, $.parseJSON(data));
                $.each(keywordCounts.data, function (i, item) {
                    weightedKeywordCounts.data[i] = [];
                    $.each(item, function (k, element) {
                        /* Trouble */
                        var weightedResult = $.extend(true, {}, element);
                        weightedResult.Count = element.Count / totalCounts.data[i][k].Count;
                        weightedResult.Count = weightedResult.Count * 100;
                        weightedResult.Count = Math.round(weightedResult.Count * 100) / 100;
                        weightedKeywordCounts.data[i][k] = weightedResult;
                    });
                });
            }
        });
        ajaxRequests.push(totalCountRequest);
        $.when(keywordCountRequest && totalCountRequest).then(function() {
            table.Draw();
            graph.Draw();
        });
    } else {
        $.when(keywordCountRequest).then(function() {
            table.Draw();
            graph.Draw();
        });
    }
    
    
};

// Clear all entered parameters, reset all fields
NewsFreq.prototype.Form.prototype.clear = function() {
    $('#DateFrom').removeClass('invalid');
    $('#DateTo').removeClass('invalid');
    $('#SearchTerms').removeClass('invalid');
    $("#SearchButton").prop('value', 'Search');
    $('input[name=searchIncrement]').removeAttr('disabled');
    $('input[name=searchWeight]').removeAttr('disabled');
    $('input[name=sourceType]').removeAttr('disabled');
    $('.DatePicker').removeAttr('disabled').removeClass('disabled');
    $('#ClearButton').prop('disabled', 'disabled').addClass('disabledButton');

    window.history.replaceState(null, '', '/');
};


////////////// Table class //////////////
NewsFreq.prototype.Table = function (newsFreq, searchData) {
    this.keywordCounts = searchData.keywordCounts;
    this.totalCounts = searchData.totalCounts;
    this.weightedKeywordCounts = searchData.weightedKeywordCounts;
    this.searchData = searchData;
    this.newsFreq = newsFreq;
};

NewsFreq.prototype.Table.prototype.Draw = function () {
    var searchWeighted = this.searchData.searchSettings.searchWeighted;
    var keywordCounts = this.keywordCounts;

    if (searchWeighted) {
        keywordCounts = this.weightedKeywordCounts;
    }

    // Draw table
    var tblBody = '<tr class="newsTableHead"><td>Keyword</td><td>Source</td><td>From</td><td>To</td><td>Instances</td></tr>';
    $.each(keywordCounts.data, function(name, results) {
        $.each(results, function (r, resultsContents) {
            var tblRow = "";
            tblRow += "<td>" + resultsContents.SearchString + "</td>";
            tblRow += resultsContents.SearchSource !== null ? "<td>" + resultsContents.SearchSource + "</td>" : "<td></td>";
            tblRow += "<td>" + getDateString(resultsContents.DateFrom) + "</td>";
            tblRow += "<td>" + getDateString(resultsContents.DateTo) + "</td>";
            tblRow += "<td>" + resultsContents.Count.toString();
            if (searchWeighted) {
                tblRow += "%";
            } 
            tblRow += "</td>";
            tblBody += "<tr>" + tblRow + "</tr>";
        });
    });
    $("#NewsFreqTableContent").html(tblBody);
    $('#NewsFreqTable').show(); // Table starts hidden when unpopulated
    
};

NewsFreq.prototype.Table.prototype.clear = function() {
    $('#NewsFreqTable').hide();
};


////////////// Graph class //////////////
NewsFreq.prototype.Graph = function (newsFreq, searchData, table) {
    this.keywordCounts = searchData.keywordCounts;
    this.totalCounts = searchData.totalCounts;
    this.searchSettings = searchData.searchSettings;
    this.searchData = searchData;
    this.table = table;
    this.newsFreq = newsFreq;
    var ctx = $("#NewsFreqGraph").get(0).getContext("2d");
    this.chart = new Chart(ctx);
};

NewsFreq.prototype.Graph.prototype.Draw = function () {
    var keywordCounts = this.keywordCounts;

    if (keywordCounts.data.length == 0) {
        this.newsFreq.clear();
        return;
    }
    
    if (this.searchData.searchSettings.searchWeighted) {
        keywordCounts = this.searchData.weightedKeywordCounts;
    }

    var table = this.table;
    var graph = this;

    if (this.searchSettings.weighted != null) {
        // Do weighted stuff
    }

    var searchData = this.searchData;
    var searchString;
    var searchSource;
    var searchKeywordColors = this.searchData.searchKeywordColors;
    // Drop all dateFrom strings into results labels array
    var chartLabels = new Array();
    $.each(keywordCounts.data[0], function (r, v) {
        chartLabels.push(getDateString(v.DateFrom));
    });

    var chartQuantData = new Array();

    // Process all the results data into the data array
    $.each(keywordCounts.data, function (a, b) {
        var resultsCount = new Array();
        $.each(b, function (r, v) {
            resultsCount.push(v.Count);
            searchString = v.SearchString;
            searchSource = v.SearchSource;
        });

        //Check if keyword already exists in searchKeywordColors and is already associated with a color, else make a new one
        var index = findElemByKeywordSource(searchKeywordColors, searchString, searchSource);
        var color = "#000"; //Default color to black
        if (index !== -1) {
            color = searchKeywordColors[index].color;
        } else {
            color = getColor(searchString, searchSource);
        }

        // Add point data to data set
        chartQuantData.push(
            {
                //label: searchString,
                fillColor: "rgba(220,220,220,0.0)",
                strokeColor: color,
                pointColor: color,
                pointStrokeColor: color,
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
    if (searchData.searchSettings.searchWeighted === false) {
        lineChartOptions = {
            bezierCurve: true,
            pointDot: true,
            scaleFontSize: 10,
            pointDotRadius: 3,
            //legendTemplate : "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].lineColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>"
        };
    } else {
        lineChartOptions = {
            bezierCurve: true,
            pointDot: true,
            pointDotRadius: 3,
            scaleFontSize: 10,
            scaleLabel: "<%=value+'%'%>",
            //legendTemplate: "<ul class=\"<%=name.toLowerCase()%>-legend\"><% for (var i=0; i<datasets.length; i++){%><li><span style=\"background-color:<%=datasets[i].lineColor%>\"></span><%if(datasets[i].label){%><%=datasets[i].label%><%}%></li><%}%></ul>"
        };
    }


    // Create chart on canvas
    
    var lineChart = this.chart;
    //var lineChart = new Chart(ctx).Line(lineChartData, lineChartOptions);
    lineChart.Line(lineChartData, lineChartOptions);
    
    // Draw legend
    drawLegend(this.keywordCounts, searchData.totalCounts, searchData.weightedKeywordCounts);
    //$("#NewsFreqGraphLegend").html(lineChart.generateLegend());
    
    $('#NewsFreqGraph').show(); //Chart starts hidden when unpopulated
    
    var labelText = 'Articles Per {1} Featuring Keyword';
    
    if (searchData.searchSettings.searchIncrement == 'Monthly') {
        labelText = labelText.replace('{1}', 'Month');
    } else if (searchData.searchSettings.searchIncrement == 'Annual') {
        labelText = labelText.replace('{1}', 'Year');
    }
    
    $('#NewsFreqGraphLabel').text(labelText).show();
    
    /* Helper functions */
    //////////////////////////////////////////////////
    //Retrieve a color from the color array and remove that index
    function getColor (keyword, source) {
        var color = "#" + searchData.colorValues[0];
        searchData.colorValues.splice(0, 1);
        searchKeywordColors.push({
            keyword: keyword,
            color: color,
            source: source
        });
        return color;
    };

    // Find index with attribute in array

    function findElemByKeywordSource (results, keyword, source) {
        for (var i = 0; i < results.length; i += 1) {
            if (results[i]['keyword'] === keyword && results[i]['source'] === source) {
                return i;
            }
        }
        return -1;
    };
    

    function drawLegend (keywordCounts, totalCounts, weightedKeywordCounts) {
        $('#NewsFreqGraphLegend').empty();
        $.each(searchKeywordColors, function (a, b) {
            var source = "";
            var sourcePrefix = "";
            if (b.source != null) {
                source = b.source;
                sourcePrefix = ":&nbsp";
            }
            $('#NewsFreqGraphLegend').append(
                '<span class="NewsFreqGraphLegendCell" data-source="'
                    + source.replace(/ /g, '&nbsp')
                    + '" data-keyword="'
                    + b.keyword
                    + '"><span style="background-color:'
                    + b.color
                    + ';">&nbsp&nbsp&nbsp&nbsp&nbsp</span>&nbsp'
                    + b.keyword.replace(/ /g, '&nbsp')
                    + sourcePrefix
                    + source
                    + '&nbsp&nbsp </span>'
            );
        });
        $('.NewsFreqGraphLegendCell').click(function () { removeVariable($(this).data('keyword')); });

        function removeVariable(keyword) {
            searchKeywordColors.splice(findWithAttr(searchKeywordColors, 'keyword', keyword), 1);
            $.each(keywordCounts.data, function (a, b) {
                if (keyword == b[0].SearchString) {
                    keywordCounts.data.splice(a, 1);
                    return false;
                }
            });
            if (searchData.searchSettings.searchWeighted) {
                $.each(totalCounts.data, function (a, b) {
                    if (keyword == b[0].SearchString) {
                        totalCounts.data.splice(a, 1);
                        return false;
                    }
                });
                $.each(weightedKeywordCounts.data, function (a, b) {
                    if (keyword == b[0].SearchString) {
                        weightedKeywordCounts.data.splice(a, 1);
                        return false;
                    }
                });
            }
            
            table.Draw();
            graph.Draw();
        }
    };

};

NewsFreq.prototype.Graph.prototype.clear = function () {
    //this.searchData = this.newsFreq.newsFreqSearchData;
    $('#ErrorLabel').text('');
    $('#NewsFreqGraph').hide();
    $('#NewsFreqGraphLegend').empty();
    $('#NewsFreqGraphLabel').hide();
};


////////////// Helpers //////////////
function getDateString(jsonDate) {
    var date = new Date(parseInt(jsonDate.substr(6)));
    return (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
};

function findWithAttr(array, attr, value) {
    for (var i = 0; i < array.length; i += 1) {
        if (array[i][attr] === value) {
            return i;
        }
    }
    return -1;
};

// QueryString jquery plugin
(function($) {
    $.QueryString = (function(a) {
        if (a == "") return {};
        var b = {};
        for (var i = 0; i < a.length; ++i)
        {
            var p=a[i].split('=');
            if (p.length != 2) continue;
            b[p[0]] = decodeURIComponent(p[1].replace(/\+/g, " "));
        }
        return b;
    })(window.location.search.substr(1).split('&'))
})(jQuery);

trimArray = function (array, goalLength) {
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
            } else j += 1;
        }
        b[m2 + 1] = n2 + 1;

        for (var i = 0; i < array.length; i++) {
            if (b.indexOf(array[i]) == -1) {
                array[i] = '';
            }
        }
    }
    return array;
};