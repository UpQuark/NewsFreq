
function sendServerQuery() {
    var params = JSON.stringify({
        dateFrom: "01/02/1990",
        dateTo: "02/02/2013",
        dateString: "January 02 1990 to February 2 2013",
        searchString: "News",
        searchTarget: "",
        createdDate: "October 3 2013"
    });

    $.ajax({
        type: "POST",
        url: "NewsLibrary.aspx/Query",
        dataType: "json",
        data: params,
        contentType: "application/json; charset=utf-8",
        async: true,
        success: function (msg) {
            $('#crap').text("HelloHelloHelloHelloHelloHelloHello");
        }
    });
    
}