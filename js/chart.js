function jsObjectGet(p_obj) {
    var v_obj;
    if (typeof p_obj == 'string') {

        v_obj = document.getElementById(p_obj);
        
    } else {
        v_obj = p_obj;
    }
    return v_obj;
}

function jsTrim(p_string) {
    return p_string.replace(/^\s*/, '').replace(/\s*$/, '');
}

function capitalizeFirstLetter(string) {
    return string.charAt(0).toUpperCase() + string.slice(1);
}

function OfflineChartsGet(userKey, chartTables, localServerName) {


    //make sure we have a table id
    if (chartTables == null || chartTables == '') {
        alert('The chartTables parameter is NULL');
    }
        //build a list of table ids
    else {
        var v_IDs = chartTables.split(',');
        
        if (v_IDs == null || v_IDs.length == 0) {
            alert('The chartTables parameter was empty');
        }
            //build a string of data and properties for each chart
        else {
            for (var c = 0; c < v_IDs.length; c++) {
                var v_dataString = '';
                var v_tableID = v_IDs[c];
                var v_table = null;
               
                if (v_tableID != null && v_tableID != '') {
                    v_table = jsObjectGet(v_tableID);

                    var tableName = "#" + v_tableID;

                   //get the location of the properties for the chart
                    var chartProperties = "#" + v_tableID + " th.chartProperties";

                    //count of the columns in the chart
                    var columnCount = $(chartProperties).data("columncount");
                 
                    var props = "";
                    var height = v_tableID == "chartData91" ? "600" : "350";
                    props = "ChartTitle1:" + $(chartProperties).data("charttitle") + "| ";
                    props += "ChartTitle2:" + $(chartProperties).data("subtitle") + "| ";
                    props += "ChartTitle3:" + $(chartProperties).data("primaryyaxistext") + "| ";
                    props += "ChartTitle4:" + $(chartProperties).data("secondaryyaxistext") + "| ";
                    props += "ChartHeight:" + height + "| ";  //hard coded because we don't have this data in the table, since highcharts works this out dynamically
                    props += "ChartWidth:735" + "| ~";
                                       

                    //is there a table to use?
                    if (v_table != null && v_table.rows.length > 1) {

                        var seriesCollection = "";

                        //loop through each Series ROW
                        for (var row = 0; row < v_table.rows.length; row++) {
                            if (seriesCollection.length > 0) {
                                seriesCollection += ';';
                            }

                            //get the column Titles just once
                            if (row == 0) {
                                
                                var colTitles = "";
                                for (var col = 1; col <= columnCount; col++) {
                                    if (col > 1) {
                                        colTitles += '~';
                                    }
                                    colTitles += jsTrim(v_table.rows[row].cells[col].innerHTML);
                                }
                            }
                            
                            //get the series data for each Series (rows above 0 are series data)
                            if (row > 0) {
                            var seriesProps = "";
                            var SeriesName = "SeriesName:" + $(tableName).find('tr:eq(' + row + ') td:eq(0)').text();
                            var SeriesType = "SeriesType:" + capitalizeFirstLetter($(tableName).find('tr:eq(' + row + ') td[id$="ChartType"]').text());
                            var SeriesColor = "SeriesColor:" + $(tableName).find('tr:eq(' + row + ') td[id$="SeriesColor"]').text();
                            var SeriesAxis = "SeriesAxis:" + $(tableName).find('tr:eq(' + row + ') td[id$="YAxisNumber"]').text();
                            SeriesAxis = SeriesAxis.replace("0", "Primary").replace("1", "Secondary");
                            var SeriesVisible = "SeriesVisible:True";
                            var SeriesHideZeros = "SeriesHideZeros:True";
                            
                                //build one string of the variables
                            seriesProps = SeriesName + "| " + SeriesType + "| " + SeriesColor + "| " + SeriesAxis + "| " + SeriesVisible + "| " + SeriesHideZeros + "| ";
                            

                          //get the series VALUES for this series
                            var oneSeries = "";
                            for (var cell = 1; cell <= columnCount; cell++) {

                                if (cell > 0) {
                                    oneSeries += '~';
                                }
                                oneSeries += jsTrim(v_table.rows[row].cells[cell].innerHTML);
                            }
                            seriesCollection += seriesProps + oneSeries;
                            }
                        }
                        if (SeriesType == 'SeriesType:Stacked' || SeriesType == 'SeriesType:Doughnut' || SeriesType == 'SeriesType:Pie') { seriesCollection = seriesCollection.split(';').reverse().join(';'); }
                        if (SeriesType != 'SeriesType:Doughnut' && SeriesType != 'SeriesType:Pie') { seriesCollection = seriesCollection.replace(/~0\.00/g, "~0.0000000001"); }
                        v_dataString = props + colTitles + ';' + seriesCollection;
                    }
                    //console.log("Dashboard v_dataString:" + v_dataString);
                }

                if (v_table != null && v_table.rows.length > 1) {
                    //console.log("calling ChartBuilder.cshtml");
                    var path = "https://" + localServerName;
                  $.post(path + '/content/Charts/ChartBuilder.cshtml?userKey=' + userKey.toString() + '&chartID=' + v_tableID + '&data=' + encodeURIComponent(v_dataString));
                } 
            }
        }
    }
}
