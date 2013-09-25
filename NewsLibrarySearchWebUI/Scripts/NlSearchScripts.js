
function searchClick() {
    var searchTerm = $("#searchText").val();
    //alert(searchTerm);
    addSearch(searchTerm);
}

//Black magic happens here
function addSearch(searchTerm) {
    var xpath = "";
    var xpath1 = "//b[contains(.,'Search Hint: ')]";
    var xpath3 = "//span[@class='basic-text-white']";
    var query1 = 'SELECT * FROM html WHERE url="http://nl.newsbank.com/nl-search/we/Archives?p_product=NewsLibrary"';
    var query = 'SELECT * FROM html WHERE url="http://nl.newsbank.com/nl-search/we/Archives/?s_siteloc=NL2&p_queryname=4000&p_action=search&p_product=NewsLibrary&p_theme=newslibrary2&s_search_type=customized&d_sources=location&d_place=United%20States&p_nbid=&p_field_psudo-sort-0=psudo-sort&f_multi=&p_multi=&p_widesearch=smart&p_sort=YMD_date%3aD&p_maxdocs=200&p_perpage=10&p_text_base-0=SEARCHTERM&p_field_base-0=&p_bool_base-1=AND&p_text_base-1=&p_field_base-1=Section&p_bool_base-2=AND&p_text_base-2=&p_field_base-2=&p_text_YMD_date-0=April_1_2001_to_April_1_2012&p_field_YMD_date-0=YMD_date&p_params_YMD_date-0=date%3aB,E&p_field_YMD_date-3=YMD_date&p_params_YMD_date-3=date%3aB,E&Search.x=18&Search.y=18" and xpath="'+xpath+'"';
    var url = "http://query.yahooapis.com/v1/public/yql?q=" + query + "&format=json&callback=??";
    //alert(url);
    $.getJSON(url, function (data) {
        alert(data.query.results);
    });
}


function doStuffOnComplete(xhr, status) {
    var response = xhr.responseText;
    $("#crap").append("Hello");
    $("#crap").append(response);
}

function createBody(searchTerm) {
    var FieldBase = "&p_field_base-0=";
    var DateBase = "&p_text_YMD_date-0=";

    var date = "April_1_2001_to_April_1_2012";
    var fieldTarget = "";
    var header = "?s_siteloc=NL2&p_queryname=4000&p_action=search&p_product=NewsLibrary&p_theme=newslibrary2&s_search_type=customized&d_sources=location&d_place=United+States&p_nbid=&p_field_psudo-sort-0=psudo-sort&f_multi=&p_multi=";
    var body = "";
    var footer = "&p_widesearch=smart&p_sort=YMD_date%3AD&p_maxdocs=200&p_perpage=10&p_text_base-0=" + searchTerm + FieldBase + fieldTarget + "&p_bool_base-1=AND&p_text_base-1=&p_field_base-1=Section&p_bool_base-2=AND&p_text_base-2=" + "&p_field_base-2=" + DateBase + date + "&p_field_YMD_date-0=YMD_date&p_params_YMD_date-0=date%3AB%2CE&p_field_YMD_date-3=YMD_date&p_params_YMD_date-3=date%3AB%2CE&Search.x=18&Search.y=18";
    return header + body + footer;
}