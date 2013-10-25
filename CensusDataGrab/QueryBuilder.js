function queryUrlBuilder(query) {
    return buildUrl(query);
}

function buildUrl(query) {
    var dataSet = query.dataSet;
    var year = query.year;
    var type = query.yype;
    var displayName = query.displayName;
    var key = query.key;
    var vars = query.vars;
    var geoIDs = query.geoIDs;

    var idCount = geoIDs.Count - 1;

    var stem = "http://api.census.gov/data/";

    var url = Stem + year + "/" + dataSet + "?" + "key=" + key + "&get=";
}