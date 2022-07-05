

function loadReport(ReportName) {

    var ReportPage = '~/BI/' + ReportName + '.html';
    //console.log(ReportPage);

    $("#reportContent").append("<p>Test</p>");


    //$.get(ReportPage, function (data) {
    //    console.log("get function was called");
    //    $("#reportContent").html($(data).find("body"));

    //console.log("Load was performed.");

};

function Reload() {
    $('#dashboardNavBar').hide();
    $("#content").html('<h2 class="text-center" style="font-size:40px;"><i id="spinner" class="fa fa-circle-o-notch fa-spin" style="font-size:40px;color:#5e5e5e;"></i> Loading...</h2> ');            //save the tree node ID in UI_Parameters
    window.location = "Index.cshtml";

}

function ShowChart(chartDivID) {
    //shows all the charts for the same kpi
    if (chartDivID == 'chart6110' || chartDivID == 'chart6120') {
        $("#" + chartDivID.slice(0, -1)).toggle('slow');
        $("#" + chartDivID.slice(0, -1) + '1').toggle('slow')
        setTimeout(function () {
            $("#" + chartDivID.slice(0, -1)).highcharts().reflow();
            $("#" + chartDivID.slice(0, -1) + '1').highcharts().reflow();
        }, 650);
    } else if (chartDivID == 'chart81') {
        $("#" + chartDivID + '0').toggle('slow')
        setTimeout(function () {
            $("#" + chartDivID + '0').highcharts().reflow()
        }, 650);
    }
    //shows or hides a chart
    $("#" + chartDivID).toggle('slow')
    setTimeout(function () {
        $("#" + chartDivID).highcharts().reflow()
    }, 650);
}

function ChartBuilder(chartID, chartKey, chartDataTable) {
    //Builds a chart on a kpi section
    //console.log(" chartBuilder: chartID: " + chartID + ",  chartKey: " + chartKey + ", chartDataTable: " + chartDataTable);

    if (chartKey == 611 || chartKey == 612) {
        HalfDoughnutBuilder(chartID + '0', chartKey + '0', chartDataTable + '0')
    } else if (chartKey == 81) {
        ChartBuilder(chartID + '0', chartKey + '0', chartDataTable + '0')
    }
    //assign values to char variables
    {
        var chartTitle = $(chartDataTable + " .chartProperties").attr("data-charttitle");
        var chartSubTitle = $(chartDataTable + " .chartProperties").attr("data-subtitle");
        var chartTitleFontWeight = $(chartDataTable + " .chartProperties").attr("data-fontweight");
        var chartTitleFontSize = $(chartDataTable + " .chartProperties").attr("data-fontsize");
        var chartPrimaryYaxisText = $(chartDataTable + " .chartProperties").attr("data-primaryyaxistext");
        var chartSecondaryYaxisText = $(chartDataTable + " .chartProperties").attr("data-secondaryyaxistext");
        var chartXAxisType = $(chartDataTable + " .chartProperties").attr("data-xaxistype");
        var chartAxisFontSize = $(chartDataTable + " .chartProperties").attr("data-xaxisfontsize");
        var chartColor = $(chartDataTable + " .chartProperties").attr("data-color");
        var chartBorderRadius = parseInt($(chartDataTable + " .chartProperties").attr("data-borderradius"));
        var chartBorderWidth = parseInt($(chartDataTable + " .chartProperties").attr("data-borderwidth"));
        var chartColumnCount = parseInt($(chartDataTable + " .chartProperties").attr("data-columncount"));
        var chartswitchRowsAndColumns = $.parseJSON($(chartDataTable + " .chartProperties").attr("data-switchcolumnandrows").toLowerCase());
        var chartCreditsEnabled = $.parseJSON($(chartDataTable + " .chartProperties").attr("data-creditsenabled").toLowerCase());
        var chartSecondaryYAxisOpposite = $.parseJSON($(chartDataTable + " .chartProperties").attr("data-secondaryyaxisopposite").toLowerCase());
        var chartLegendEnabled = $.parseJSON($(chartDataTable + " .chartProperties").attr("data-legendenabled").toLowerCase());
        var chartExportButtonEnabled = $.parseJSON($(chartDataTable + " .chartProperties").attr("data-exportbuttonenabled").toLowerCase());
        
        var rounding = $('#selectedRounding').text() == 'Thousands' ? 'K' : 'M'

        //console.log("chartTitle: " + chartTitle + ", chartSubTitle :" + chartSubTitle + ", chartTitleFontWeight: " + chartTitleFontWeight + ", chartTitleFontSize: " + ", chartPrimaryYaxisText:" + chartPrimaryYaxisText + ", chartSecondaryYaxisText:" + chartSecondaryYaxisText);
        //console.log("chartXAxisType: " + chartXAxisType + ", chartAxisFontSize :" + chartAxisFontSize + ", chartColor: " + chartColor + ", chartBorderRadius: " + chartBorderRadius + ", chartBorderWidth:" + chartBorderWidth + ", chartColumnCount:" + chartColumnCount);
        //console.log("chartswitchRowsAndColumns: " + chartswitchRowsAndColumns + ", chartCreditsEnabled :" + chartCreditsEnabled + ", chartSecondaryYAxisOpposite: " + chartSecondaryYAxisOpposite + ", chartLegendEnabled: " + chartLegendEnabled + ", chartBorderWidth:" + chartBorderWidth + ", chartExportButtonEnabled:" + chartExportButtonEnabled);
        //console.log("chartSecondaryYAxisOpposite:" + chartSecondaryYAxisOpposite);

        //Series variables
        var seriesCount = $('#chartData' + chartKey + ' > tbody > tr').size() - 1;
        chartKey = "#ID" + chartKey;
        var series = {};
        var isStacked, isPie;

        for (var i = 1; i <= seriesCount; i++) {
            series[i] = {};
            series[i]['yAxisNumber'] = parseInt($(chartKey + i + "-YAxisNumber").text(), 10);
            series[i]['chartType'] = $(chartKey + i + "-ChartType").text();
            if (series[i]['chartType'] == "stacked") { isStacked = true; series[i]['chartType'] = "column"; }
            if (series[i]['chartType'] == "pie") { isPie = true; }
            series[i]['color'] = $(chartKey + i + "-SeriesColor").text();
            series[i]['dataType'] = $(chartKey + i + "-DataType").text();
            series[i]['showVariance'] = $(chartKey + i + "-ShowVariance").text();
        }
        //Chart Variables (light grey)
        var borderColor = "#ddd";
    }

    //build the chart attributes
    {
        //Title
        if (chartTitle != "") {
            var objTitle = {
                text: chartTitle,
                style: {
                    color: chartColor,
                    fontWeight: chartTitleFontWeight,
                    fontSize: chartTitleFontSize
                }
            }
        } else { var objTitle = ""; }


        //SubTitle
        if (chartSubTitle != "") {
            var objSubtitle = {
                text: chartSubTitle,
                style: {
                    color: '#888b8d'
                }
            }
        } else { var objSubtitle = ""; }


        //Yaxis
        if (chartSecondaryYaxisText != "") {
            //build two axis
            var objYaxis =
                //--- Primary yAxis
                 [
                    {
                        title: {
                            text: chartPrimaryYaxisText,
                            style: {
                                color: chartColor
                            }
                        },
                        labels: {
                            formatter: function () {
                                if (this.value % 1 != 0 && chartPrimaryYaxisText != "Percent") {
                                return Highcharts.numberFormat(this.value,1);
                            } else {
                                return Highcharts.numberFormat(this.value,0);
                            }},
                            style: {
                                fontSize: chartAxisFontSize,
                                color: chartColor
                            }
                        },
                        softMin: 0
                    },

                    // Secondary yAxis
                    {
                        title: {
                            text: chartSecondaryYaxisText,
                            style: {
                                color: chartColor
                            }
                        },
                        opposite: chartSecondaryYAxisOpposite,
                        labels: {
                            format: "{value:,.0f}",
                            style: {
                                fontSize: chartAxisFontSize,
                                color: chartColor
                            }
                        },
                        softMin: 0
                     }
                 ]
        }
        else {
            //Build only 1 Yaxis on the left side
            var maxY = (chartKey == '#ID74' || chartKey == '#ID63' || chartKey == '#ID65') ? 100 : null //For the timesheets compliance graph, ensures that the percent axis max out at 100
            var objYaxis = {
                    title: {
                        text: chartPrimaryYaxisText,
                        style: {
                            color: chartColor
                        }
                    },
                    labels: {
                        formatter: function () {
                            if (this.value % 1 != 0 && chartPrimaryYaxisText != "Percent") {
                                return Highcharts.numberFormat(this.value, 1);
                            } else {
                                return Highcharts.numberFormat(this.value, 0);
                            }
                        },
                        style: {
                            fontSize: chartAxisFontSize,
                            color: chartColor
                        }
                    },
                    ceiling: maxY
                }
            }

        //stacked column option
        if (isStacked) {
            var objPlotOption =
                {
                    series: {
                        stacking: 'normal',
                        marker: {
                            fillColor: null,
                            lineWidth: 1,
                            lineColor: null, // inherit from series
                            symbol: "circle"
                        }
                    }
                };
        }
        else if (isPie) {
            var objPlotOption =
                {
                    pie: {
                        colors: ['#1b365d','#007681','#71b2b9','#c8c9c7','#e87722','#82c992'],
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            format: '{point.name}: {point.percentage:.1f} %',
                            style: {
                                color: 'grey',
                                fontWeight: 'normal'
                            }
                        }
                    }
                };
        }
        else {
            objPlotOption =
                {
                    series: {
                        stacking: null,
                        marker: {
                            fillColor: null,
                            lineWidth: 1,
                            lineColor: null, // inherit from series
                            //symbol: "undefined"
                        }
                    }
                }   
        }

        //Series
        var objSeries = [];

        if (isPie) {
            objSeries = [{
                name: 'Amount',
                type: 'pie',
                colorByPoint: true
            }]
        } else {
            for (var i = 1; i <= seriesCount; i++) {
                objSeries.push({
                    yAxis: series[i]['yAxisNumber'],
                    type: series[i]['chartType'],
                    color: series[i]['color']
                })
            }
        }

        //xAxis labels (these come from th row in chartDataTable)
        var objXaxis = {
            type: chartXAxisType,
            labels: {
                style: {
                    fontSize: chartAxisFontSize,
                    color: chartColor
                }
            }
        }

        //data source
        //chartDataTable contains # prefix. Remove it
        chartDataTable = chartDataTable.replace("#", '');

        var objData = {
            table: chartDataTable,
            switchRowsAndColumns: chartswitchRowsAndColumns,
            endColumn: chartColumnCount   //columns after this one are meta data about the series
        }

        //Chart Style
        var size = (chartKey == '#ID6111' || chartKey == '#ID6121') ? 300 : null
        var objChartStyle = {
            height: size,
            borderColor: borderColor,
            borderRadius: chartBorderRadius,
            borderWidth: chartBorderWidth
        }

        //Legend 
        var objLegend = {
            enabled: chartLegendEnabled,
            itemStyle: {
                color: chartColor,
                fontWeight: "normal"
            },
            symbolRadius: 0
        }

        //Export Button
        var objExport = {
            buttonOptions: {
                enabled: chartExportButtonEnabled  //enable export button
            }
        }

        //Credits enabled
        var objCredits = {
            enabled: chartCreditsEnabled
        }
    }

    //construct the chart

    //try {
        $(chartID).highcharts(
        {
            //Credits
            credits: objCredits,

            //data source
            data: objData,

            //chart style options
            chart: objChartStyle,

            //chart Title
            title: objTitle,

            //chart subtitle
            subtitle: objSubtitle,

            //Y Axis
            yAxis: objYaxis,

            plotOptions: objPlotOption,

            //data series
            series: objSeries,

            //xAxis labels 
            xAxis: objXaxis,

            //Chart Legend
            legend: objLegend,

            //Export Button
            navigation: objExport,
            exporting: {
                buttons: {
                    contextButton: {
                        menuItems: [{
                            text: 'Export as image',
                            onclick: function () {
                                this.exportChart();
                                $.post("/content/controls/UserActivityLog_Set.cshtml?ActivityType=Export Chart&ActivityName=" + this.title.textStr)
                            }
                        }]
                    }
                },
                filename: chartTitle,
                type: "image/jpeg"
            },

            //tooltip
            tooltip: {
                formatter: function () {
                    if (series[this.series.index + 1]['dataType'] == "Percent") { //tooltip format for percent series
                        var tooltip = '', change = '';
                        if (series[this.series.index + 1]['showVariance'] == "True") { //adds a line that shows the variance with LY
                            var pointIndex = this.point.index;
                            var CY = this.point.y;
                            var LY = $(chartID).highcharts().series[this.series.index - 1].data[pointIndex].y;
                            var diff = Math.round((CY - LY) * 10) / 10;
                            change = diff > 0 ? '+' + diff : diff
                            change = 'Var. ' + change + 'pts</b><br/>';
                        }
                        tooltip += '<b>' + this.series.name + '</b><br/>';
                        tooltip += Highcharts.numberFormat(this.y, 1) + '%</b><br/>';
                        tooltip += change
                        tooltip += ' ' + this.point.name;
                        return tooltip;
                    }
                    else {
                        var message = '', variance = '';
                        if (chartKey == '#ID91') { //special case for New Services Estimate
                            var decimals = rounding == 'K' ? 0 : 2 //show two decimals for rounding in M
                            tooltip = this.series.name + ': ' + Highcharts.numberFormat(this.y, decimals) + ' ' + rounding;
                        }
                        else if (chartKey == '#ID71') {
                            var pointIndex = this.point.index;
                            var total = $(chartID).highcharts().series.reduce(function (a, b) { return a + b.data[pointIndex].y }, 0)
                            tooltip = '<b>Total: ' + Math.round(total) + '</b><br /><br />' + this.series.name + ': ' + Highcharts.numberFormat(this.y, 0)
                        }
                        else if (rounding == 'M' && series[this.series.index + 1]['dataType'] == 'Amount') { //show two decimals for rounding in M
                            tooltip = this.series.name + ': ' + Highcharts.numberFormat(this.y, 2);
                        }
                        else { //default tooltip
                            tooltip = this.series.name + ': ' + Highcharts.numberFormat(this.y, 0) + '<br /><br />'
                        }
                        if (series[this.series.index + 1]['showVariance'] == "True") { //adds a line that shows the variance with LY
                            var pointIndex = this.point.index;
                            var CY = this.point.y;
                            var LY = $(chartID).highcharts().series[0].data[pointIndex].y;
                            var diff = Math.round(((CY - LY) / LY) * 1000) /10;
                            diff = diff > 0 ? '+' + diff : diff
                            if (diff == '+Infinity') { variance = '</b><br/>Var. -'; }
                            else { variance = '</b><br/>Var. ' + diff + '%' };
                        }
                        return tooltip + variance
                    }
                }
            }
            
        });
}

function HalfDoughnutBuilder(chartID, chartKey, chartDataTable) {
    var chartTitle = $(chartDataTable + " .chartProperties").attr("data-charttitle");
    var chartSubTitle = $(chartDataTable + " .chartProperties").attr("data-subtitle");
    var chartTitleFontWeight = $(chartDataTable + " .chartProperties").attr("data-fontweight");
    var chartTitleFontSize = $(chartDataTable + " .chartProperties").attr("data-fontsize");
    var chartPrimaryYaxisText = $(chartDataTable + " .chartProperties").attr("data-primaryyaxistext");
    var chartSecondaryYaxisText = $(chartDataTable + " .chartProperties").attr("data-secondaryyaxistext");
    var chartXAxisType = $(chartDataTable + " .chartProperties").attr("data-xaxistype");
    var chartAxisFontSize = $(chartDataTable + " .chartProperties").attr("data-xaxisfontsize");
    var chartColor = $(chartDataTable + " .chartProperties").attr("data-color");
    var chartBorderRadius = parseInt($(chartDataTable + " .chartProperties").attr("data-borderradius"));
    var chartBorderWidth = parseInt($(chartDataTable + " .chartProperties").attr("data-borderwidth"));
    var chartColumnCount = parseInt($(chartDataTable + " .chartProperties").attr("data-columncount"));
    var chartswitchRowsAndColumns = $.parseJSON($(chartDataTable + " .chartProperties").attr("data-switchcolumnandrows").toLowerCase());
    var chartCreditsEnabled = $.parseJSON($(chartDataTable + " .chartProperties").attr("data-creditsenabled").toLowerCase());
    var chartSecondaryYAxisOpposite = $.parseJSON($(chartDataTable + " .chartProperties").attr("data-secondaryyaxisopposite").toLowerCase());
    var chartLegendEnabled = $.parseJSON($(chartDataTable + " .chartProperties").attr("data-legendenabled").toLowerCase());
    var chartExportButtonEnabled = $.parseJSON($(chartDataTable + " .chartProperties").attr("data-exportbuttonenabled").toLowerCase());


    //Series variables
    var seriesCount = $('#chartData' + chartKey + ' > tbody > tr').size() - 1;
    chartKey = "#ID" + chartKey;
    var series = {};

    for (var i = 1; i <= seriesCount; i++) {
        series[i] = {};
        series[i]['yAxisNumber'] = parseInt($(chartKey + i + "-YAxisNumber").text(), 10);
        series[i]['chartType'] = $(chartKey + i + "-ChartType").text();
        series[i]['color'] = $(chartKey + i + "-SeriesColor").text();
    }
    //Chart Variables (light grey)
    var borderColor = "#ddd";

    //build the chart attributes
    {
    //Title
    if (chartTitle != "") {
        var objTitle = {
            text: chartTitle,
            style: {
                color: chartColor,
                fontWeight: chartTitleFontWeight,
                fontSize: chartTitleFontSize
            }
        }
    } else { var objTitle = ""; }


    //SubTitle
    if (chartSubTitle != "") {
        var objSubtitle = {
            text: chartSubTitle,
            style: {
                color: '#888b8d'
            }
        }
    } else { var objSubtitle = ""; }


    //Yaxis
    //Build only 1 Yaxis on the left side
    var objYaxis = {
        title: {
            text: chartPrimaryYaxisText,
            style: {
                color: chartColor
            }
        },
        labels: {
            format: "{value:,.0f}",
            style: {
                fontSize: chartAxisFontSize,
                color: chartColor
            }
        },
    }

    var objPlotOption =
        {
            pie: {
                size: 275,
                colors: [series[1] ? series[1]['color'] : null, series[2] ? series[2]['color'] : null, series[3] ? series[3]['color'] : null, series[4] ? series[4]['color'] : null], 
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: false,
                    formatter: function () {
                        return this.y;
                    }
                },
                startAngle: -90,
                endAngle: 90,
                center: ['50%', '75%']
            }
        };

    //Series
    var objSeries = [{
            name: 'Count',
            type: 'pie',
            colorByPoint: true,
            innerSize: '50%'
        }]

    //xAxis labels (these come from th row in chartDataTable)
    var objXaxis = {
        type: chartXAxisType,
        labels: {
            style: {
                fontSize: chartAxisFontSize,
                color: chartColor
            }
        }
    }

    //data source
    //chartDataTable contains # prefix. Remove it
    chartDataTable = chartDataTable.replace("#", '');

    var objData = {
        table: chartDataTable,
        switchRowsAndColumns: chartswitchRowsAndColumns,
        endColumn: chartColumnCount   //columns after this one are meta data about the series
    }

    //Chart Style
    var objChartStyle = {
        height: 300,
        borderColor: borderColor,
        borderRadius: chartBorderRadius,
        borderWidth: chartBorderWidth
    }

    //Legend 
    var objLegend = {
        enabled: chartLegendEnabled,
        itemStyle: {
            color: chartColor,
            fontWeight: "normal"
        }
    }

    //Export Button
    var objExport = {
        buttonOptions: {
            enabled: chartExportButtonEnabled  //disable export button with false
        }
    }

    //Credits enabled
    var objCredits = {
        enabled: chartCreditsEnabled
    }
    }

    //construct the chart

    //try {
    $(chartID).highcharts(
    {
        //Credits
        credits: objCredits,

        //data source
        data: objData,

        //chart style options
        chart: objChartStyle,

        //chart Title
        title: objTitle,

        //chart subtitle
        subtitle: objSubtitle,

        //Y Axis
        yAxis: objYaxis,

        plotOptions: objPlotOption,

        //data series
        series: objSeries,

        //xAxis labels 
        xAxis: objXaxis,

        //Chart Legend
        legend: objLegend,

        //Export Button
        navigation: objExport,
        exporting: {
            buttons: {
                contextButton: {
                    menuItems: [{
                        text: 'Export as image',
                        onclick: function () {
                            this.exportChart();
                            $.post("/content/controls/UserActivityLog_Set.cshtml?ActivityType=Export Chart&ActivityName=" + this.title.textStr);
                        }
                    }]
                }
            },
            filename: chartTitle,
            type: "image/jpeg"
        },

        //tooltip
        tooltip: {
            headerFormat: '<span><b>{point.key}</b></span><br/>',
            pointFormat: '<span style="color:{point.color}">\u25CF</span> {series.name}: {point.y}<br/>'
        }
    });
}

function SnapShotBuilder() {

    //build the snapshot values
    //pipeline
    var pipeline = $("#ss_kpiDetail1").html();
    if (pipeline == "" || pipeline == "(No Data Found)") {
        pipeline = "N/A";
    }


    $("#ss_pipeline").html(pipeline);

    //aot
    var aot = $("#ss_kpiDetail2").html();
    if (aot == "" || aot == "(No Data Found)") {
        aot = "N/A";
    }


    $("#ss_aot").html(aot);

    //turnover
    var to = $("#ss_kpiDetail10").html();
    if (to == "" || to == "(No Data Found)") {
        to = "N/A";
    }
    $("#ss_empturnover").html(to);


    //chargeability
    var chargeability = $("#ss_kpiDetail8").html();
    if (chargeability == "" || chargeability == "(No Data Found)") {
        chargeability = "N/A";
    }

    $("#ss_chargeability").html(chargeability);

    //CSM
    var csm = $("#ss_kpiDetail15").html();
    if (csm == "" || csm == "(No Data Found)") {
        csm = "N/A";
    }
    $("#ss_csm").html(csm);

    //opbbgc
    // handled in Index document ready
    
}

function Init_KPIPage(chartDiv, KPIDetailDiv, chartDataTable, ReportKey, callback) {

    //console.log("chartDiv: " + chartDiv + ", KPIDetailDiv: " + KPIDetailDiv + ", chartDataTable: " + chartDataTable + ", ReportKey: " + ReportKey);


    //id of the show/hide chart button
    var btnChartID = "#btn-" + chartDiv;

    //ID of the div we are targeting
    var kpiSectionID = "#" + KPIDetailDiv;

    //ID of the kpivalue span
    var kpiValue = "#ss_" + KPIDetailDiv;

    //ID of the kpiGrowth span
    var kpiGrowth = "#growth_" + KPIDetailDiv;


    //ID of the Chart Data Table
    var chartDataTableID = "#" + chartDataTable;

    //console.log("chartDatTableID" + chartDataTableID);

    //chart ID
    var chartID = "#" + chartDiv;

    //chartKey
    var chartKey = ReportKey;

    //assume we have data to show
    var dataFound = true;

    //count the number of rows in the kpi table
    var rowCount = $(kpiSectionID + " table:first tbody > tr").length;

    //Look for KPI data to show. if no data, then disable the section
    if (rowCount == 0) {
        //no data found
        dataFound = false;
        if (ReportKey != "612" && ReportKey != "65" && ReportKey != "63") {
            $("div[href='" + kpiSectionID + "']").removeAttr("data-toggle").removeClass("enabled");
            $("a[href='" + kpiSectionID + "']").addClass("disabled");
            $(kpiValue).text("(No Data Found)");
            $(kpiGrowth).remove();
            $(kpiSectionID).addClass("not-expandable");
        }
    }

    //Look for chart data. If no data, don't try to build a chart and hide the show chart button
    //count the number of rows in the chart data table
    rowCount = $(chartDataTableID + " tbody > tr").length;
    if (rowCount == 0 || rowCount == 1) {
        //no chart data found
        dataFound = false;
        $(btnChartID).hide();
    }

    if (dataFound) {

        //we have some data so let's build a chart
        //Highcharts init
        //call the function
        ChartBuilder(chartID, chartKey, chartDataTableID);
        if (chartDiv == 'chart611' || chartDiv == 'chart612') {
            ChartBuilder(chartID + '1', chartKey + '1', chartDataTableID + '1');
            $(chartID + "1").hide();
            $(chartID + "0").hide();
        } else if (chartDiv == 'chart81') {
            $(chartID + "0").hide();
        }
        if(chartDiv == 'chart81') { $(chartID + "0").hide(); }
        $(chartID).hide();
    }

    // Make sure the callback is a function​
    if (typeof callback === "function") {
        // Call it, since we have confirmed it is callable​
        callback();
    }


}

function setCookie(cname, cvalue, exdays) {
    if (cvalue != "") {
        var d = new Date();
        d.setTime(d.getTime() + (exdays * 24 * 60 * 60 * 1000));
        var expires = "expires=" + d.toUTCString();
        document.cookie = cname + "=" + cvalue + "; " + expires;
    }
}

function getCookie(cname) {
    var name = cname + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) == ' ') {
            c = c.substring(1);
        }
        if (c.indexOf(name) == 0) {
            return c.substring(name.length, c.length);
        }
    }
    return "";
}

function resizeTable() {
    if ($(window).width() < 768) {
        $('.dashboard-table > thead > tr').each(function () {
            $(this).find('th:not(:first-child)').css('width', (((($(this).closest('.portlet').width() - $(this).data('width')) / $(this).closest('.portlet').width()) * 100) / $(this).find('th:not(.hidden-xs):not(:first-child)').size()) + "%");
        });
    } else {
        $('.dashboard-table > thead > tr').each(function () {

            $(this).find('th:not(:first-child)').css('width', (((($(this).closest('.portlet').width() - $(this).data('width')) / $(this).closest('.portlet').width()) * 100) / $(this).find('th:not(:first-child)').size()) + "%");
        });
    }
}

// show the clicked section and collapse the other ones
function GoToSection(SectionName) {
   // console.log("in function GoToSection");
    if (typeof jQuery === 'undefined') {
        console.log("no jquery");
    }

    if (typeof ($.fn.popover) == 'undefined') {
        console.log("no bootstrap");
    }


    var sectionID = "#" + SectionName;
    $(window).scrollTop(0);
    if ($(sectionID + '.panel-collapse').is(":visible")) {
        $(sectionID + '.panel-collapse').collapse('hide');
    } else {
        $('.panel-collapse.in:not(' + sectionID + ')').collapse('hide');
        $(sectionID + '.panel-collapse').collapse('show');
    }
    resizeTable();

    if (SectionName === "portlet-winning") {
        console.log("portlet-winning was called");
        
    }
}

// show the clicked kpi and collapse the other sections
function GoToKPI(SectionName, kpiName) {
    var sectionID = "#" + SectionName;
    var kpiID = "#" + kpiName;
    if ($(kpiID).length != 0) {
        $('.panel-collapse.in:not(' + sectionID + ', ' + kpiID + ')').collapse('hide');
        $(sectionID + '.panel-collapse').collapse('show');
        $(kpiID + '.panel-collapse').collapse('show');
        resizeTable();
        setTimeout(function () {
            scrollToElement($(kpiID));
        }, 401);
    }
}

function scrollToElement(ele) {
    $(window).scrollTop(ele.offset().top - 120).scrollLeft(ele.offset().left);
}

//open / close the sections
function expand_all(trigger) {
    if (trigger === 'KPI') {
        $('.wraper .collapse').not(".not-expandable").collapse("show");
    }
    else {
        $('.wraper #portlet-' + trigger + ' .collapse.chart-container').collapse("show");
    }
    resizeTable();
}

function collapse_all(trigger) {
    if (trigger === 'KPI') {
        $('.wraper .collapse').collapse("hide");
    }
    else {
        $('.wraper #portlet-' + trigger + ' .collapse.chart-container').collapse("hide");
    }
}