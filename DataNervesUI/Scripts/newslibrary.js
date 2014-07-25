/* NewsFreq class */
///////////////////////////////////////////////////////////////////////////////
function NewsFreq() {
    
    // Search data share
    this.newsFreqSearchData = {
        resultsData: new this.Results(),
        weightData: new this.Results(),
        searchSettings: {
            searchIncrement: 'None',
            searchWeighted: false
        },
        searchKeywordColors: [],
        ajaxRequests: [],
        colorValues: [                   // Collection of colors to use in graphing
            "b3d7e0", "4564a5", "45a2a5",
            "800000", "008000", "000080", "808000", "800080", "008080", "808080",
            "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0",
            "400000", "004000", "000040", "404000", "400040", "004040", "404040",
            "200000", "002000", "000020", "202000", "200020", "002020", "202020",
            "600000", "006000", "000060", "606000", "600060", "006060", "606060",
            "A00000", "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0",
            "E00000", "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0"
        ],
    };
    
    this.table = new this.Table(this, this.newsFreqSearchData);
    this.graph = new this.Graph(this, this.newsFreqSearchData, this.table);
    this.form = new this.Form(this, this.newsFreqSearchData, this.table, this.graph);
}

NewsFreq.prototype.clear = function() {
    this.newsFreqSearchData.resultsData.clear();
    this.newsFreqSearchData.weightData.clear();

    var ajaxRequests = this.ajaxRequests;
    this.newsFreqSearchData.searchKeywordColors = [];

    // Abort all requests in progress
    if (ajaxRequests) {
        $.each(ajaxRequests, function (a, b) {
            b.abort();
        });
    }

    // Reset color values to default literal
    this.newsFreqSearchData.colorValues = [
        "b3d7e0", "4564a5", "45a2a5",
        "800000", "008000", "000080", "808000", "800080", "008080", "808080",
        "C00000", "00C000", "0000C0", "C0C000", "C000C0", "00C0C0", "C0C0C0",
        "400000", "004000", "000040", "404000", "400040", "004040", "404040",
        "200000", "002000", "000020", "202000", "200020", "002020", "202020",
        "600000", "006000", "000060", "606000", "600060", "006060", "606060",
        "A00000", "00A000", "0000A0", "A0A000", "A000A0", "00A0A0", "A0A0A0",
        "E00000", "00E000", "0000E0", "E0E000", "E000E0", "00E0E0", "E0E0E0"
    ];

    this.form.clear();
    this.graph.clear();
    this.table.clear();
    
};

/* Results class */
///////////////////////////////////////////////////////////////////////////////
NewsFreq.prototype.Results = function () { this.data = new Array(); };

// Add an additional variable array of data to results
NewsFreq.prototype.Results.prototype.addVariable = function (variable) { this.data.push(variable); };

// Erase all contents
NewsFreq.prototype.Results.prototype.clear = function () { this.data = []; };

// Is empty
NewsFreq.prototype.Results.prototype.isEmpty = function () {
    if (this.data.length == 1 && this.data[0].length == 0) {
        return true;
    }
    return false;
};



/* Form class */
///////////////////////////////////////////////////////////////////////////////

// Constructor
NewsFreq.prototype.Form = function (newsFreq, searchData, table, graph) {
    this.searchData = searchData;
    this.status = "enabled";
    this.table = table;
    this.graph = graph;
    this.ajaxRequests = new Array();
    this.newsFreq = newsFreq;


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

// Disable all form fields that should be impermeable during active searches
NewsFreq.prototype.Form.prototype.disable = function () {
    // Alter UI for in-progress search
    $("#SearchButton").prop('value', 'Add variable');
    $('input[name=searchIncrement]').attr('disabled', 'disabled');
    $('input[name=searchWeight]').attr('disabled', 'disabled');
    $('input[name=sourceType]').attr('disabled', 'disabled');
    $('.DatePicker').prop('disabled', 'disabled').addClass('disabled');
    $('#ClearButton').prop('disabled', '').removeClass('disabledButton');
};

/* Getters */
// Get parameters from form field entries
NewsFreq.prototype.Form.prototype.getParams = function (weighted, queryStringExists) {
    if (queryStringExists) {
        // Not implemented
        return null;
    } else {
        if (!weighted) {
            return {
                DateFrom: $('#DateFrom').val(),
                DateTo: $('#DateTo').val(),
                DateString: $('#DateFrom').val() + ' to ' + $('#DateTo').val(),
                SearchString: $('#SearchTerms').val(),
                SearchTarget: $('#SearchTargets').val(),
                SearchSource: $('#SearchSource').val()
            };
        } else {
            return {
                DateFrom: $('#DateFrom').val(),
                DateTo: $('#DateTo').val(),
                DateString: $('#DateFrom').val() + ' to ' + $('#DateTo').val(),
                SearchString: '',
                SearchTarget: $('#SearchTargets').val(),
                SearchSource: $('#SearchSource').val()
            };
        }
    }
};

// Get search settings from form field entries
NewsFreq.prototype.Form.prototype.getSearchSettings = function () {
    var searchSettings = {
        searchIncrement: '',
        searchWeighted: ''
    };
    
    // Check search time increment
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

    if (dateFrom == ''
        || dateFrom == null
        || (isNaN(Date.parse(dateFrom)))) {

        $('#DateFrom').addClass('invalid');
        valid = false;
    } else {
        $('#DateFrom').removeClass('invalid');
    }

    if (dateTo == ''
        || dateTo == null
        || (isNaN(Date.parse(dateTo)))) {
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

NewsFreq.prototype.Form.prototype.getQueryString = function () {
    if ($.QueryString) {
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
    return null;
};

// Fire search
NewsFreq.prototype.Form.prototype.search = function () {
    // Kill function if form validation fails and there is no queryString
    if (!this.getQueryString() && !this.validateUserInput()) {
        return;
    }

    var qstring = this.getQueryString();
    // Disable UI for in progress search
    this.disable();

    this.searchData.searchSettings = this.getSearchSettings();
    var searchWeighted = this.searchData.searchSettings.searchWeighted;
    var searchIncrement = this.searchData.searchSettings.searchIncrement;
    var resultsData = this.searchData.resultsData;
    var weightData = this.searchData.weightData;
    var table = this.table;
    var graph = this.graph;
    var ajaxRequests = this.ajaxRequests;

    // Create request params
    var params = this.getQueryString() != null ? this.getParams(false, true) : this.getParams(false, false);

    // Create weighted request params
    if (searchWeighted) {
        var weightParams = this.getQueryString() != null ? this.getParams(true, true) : this.getParams(true, false);
    }

    // Send query to API
    var keywordCountRequest = $.ajax({
        url: 'api/NewsLibrary',
        type: "POST",
        data: {
            query: params,
            searchType: searchIncrement
        },
        dataType: "json",
        success: function (data) {
            if (searchWeighted) {
                var weightCounts = $.ajax({
                    url: 'api/NewsLibrary',
                    type: "POST",
                    data: {
                        query: weightParams,
                        searchType: searchIncrement
                    },
                    dataType: "json",
                    success: function (weight) {
                        resultsData.addVariable($.parseJSON(data));
                        weightData.addVariable($.parseJSON(weight));

                        if (searchWeighted) {
                            $.each(resultsData.data, function (i, item) {
                                //weightData.data[i] = new Array();
                                //var weightDataArray = weightData;
                                $.each(item, function (k, element) {
                                    var weightedResult = $.extend(true, {}, element);
                                    weightedResult.Count = element.Count / weightData.data[i][k].Count;
                                    weightedResult.Count = weightedResult.Count * 100;
                                    weightedResult.Count = Math.round(weightedResult.Count * 100) / 100;
                                    weightData.data[i][k] = weightedResult;
                                });
                            });
                            
                        } else {
                            weightData = resultsData;
                        }
                        table.Draw();
                        graph.Draw();
                    }
                });
                ajaxRequests.push(weightCounts);
            } else {
                resultsData.addVariable($.parseJSON(data));
                table.Draw();
                graph.Draw();
            }
        }
    });
    ajaxRequests.push(keywordCountRequest);
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
};



/* Table data structure */
///////////////////////////////////////////////////////////////////////////////
NewsFreq.prototype.Table = function (newsFreq, searchData) {
    this.resultsData = searchData.resultsData;
    this.weightData = searchData.weightData;
    this.searchData = searchData;
    this.newsFreq = newsFreq;
};

NewsFreq.prototype.Table.prototype.Draw = function () {
    var searchWeighted = this.searchData.searchSettings.searchWeighted
    var resultsData = this.weightData;

    var tblBody = '<tr class="newsTableHead"><td>Keyword</td><td>Source</td><td>From</td><td>To</td><td>Instances</td></tr>';
    $.each(resultsData.data, function(name, results) {
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

function getDateString(jsonDate) {
    var date = new Date(parseInt(jsonDate.substr(6)));
    return (date.getMonth() + 1) + '/' + date.getDate() + '/' + date.getFullYear();
};

/* Graph data structure */
///////////////////////////////////////////////////////////////////////////////
NewsFreq.prototype.Graph = function (newsFreq, searchData, table) {
    this.resultsData = searchData.resultsData;
    this.weightData = searchData.weightData;
    this.searchSettings = searchData.searchSettings;
    this.searchData = searchData;
    this.table = table;
    this.newsFreq = newsFreq;
};

NewsFreq.prototype.Graph.prototype.Draw = function () {
    var resultsData = this.weightData;;
    if (resultsData.data.length == 0) {
        this.newsFreq.clear();
        return;
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
    $.each(resultsData.data[0], function (r, v) {
        chartLabels.push(getDateString(v.DateFrom));
    });

    var chartQuantData = new Array();

    // Process all the results data into the data array
    $.each(resultsData.data, function (a, b) {
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
                pointStrokeColor: color,
                data: resultsCount,
                title: searchString
            });
    });

    // Trim chartLabels to goalLength if length exceeds it
    var goalLength = 48;
    chartLabels = trimArray(chartLabels, goalLength);

    var lineChartData = {
        labels: chartLabels,
        datasets: chartQuantData
    };

    var lineChartOptions;


    //TODO: condense these into single block with amendment
    if (searchData.searchSettings.searchWeighted == false) {
        lineChartOptions = {
            bezierCurve: true,
            pointDot: false,
            scaleFontSize: 10,
        };
    } else {
        lineChartOptions = {
            bezierCurve: true,
            pointDot: false,
            scaleFontSize: 10,
            scaleLabel: "<%=value+'%'%>",
        };
    }


    // Create chart on canvas
    new Chart(document.getElementById("NewsFreqGraph").getContext("2d")).Line(lineChartData, lineChartOptions);
    drawLegend(resultsData, searchData.weightData);
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
        var color = searchData.colorValues[0];
        searchData.colorValues.splice(0, 1);
        searchKeywordColors.push({
            keyword: keyword,
            color: color,
            source: source
        });
        return "#" + color;
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
    

    function drawLegend (results, weight) {
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
                    + '"><span style="background-color: #'
                    + b.color
                    + ';">&nbsp&nbsp&nbsp&nbsp&nbsp</span>&nbsp'
                    + b.keyword.replace(/ /g, '&nbsp')
                    + sourcePrefix
                    + source
                    + '&nbsp&nbsp </span>'
            );
            $('.NewsFreqGraphLegendCell').click(function () { removeVariable($(this).data('keyword')); });
        });

        function removeVariable(keyword) {
            searchKeywordColors.splice(findWithAttr(searchKeywordColors, 'keyword', keyword), 1);
            $.each(results.data, function (a, b) {
                while (findWithAttr(b, 'SearchString', keyword) != -1) {
                    b.splice(findWithAttr(b, 'SearchString', keyword), 1);
                }
            });
            table.Draw();
            graph.Draw();
        }
        
        function findWithAttr(array, attr, value) {
            for (var i = 0; i < array.length; i += 1) {
                if (array[i][attr] === value) {
                    return i;
                }
            }
            return -1;
        };
    };

};

NewsFreq.prototype.Graph.prototype.clear = function () {
    //this.searchData = this.newsFreq.newsFreqSearchData;
    $('#ErrorLabel').text('');
    $('#NewsFreqGraph').hide();
    $('#NewsFreqGraphLegend').empty();
    $('#NewsFreqGraphLabel').hide();
};



// QueryString jquery plugin. Put somewhere else
(function ($) {
    $.QueryString = (function(a) {
        if (a == "") return null;
        var b = {};
        for (var i = 0; i < a.length; ++i) {
            var p = a[i].split('=');
            if (p.length != 2) continue;
            b[p[0]] = decodeURIComponent(p[1].replace(/\+/g, " "));
        }
        return b;
    })(window.location.search.substr(1).split('&'));
})(jQuery);

// Replaces elements of array with empty string at regular intervals until # of nonempty cells == goalLength
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
