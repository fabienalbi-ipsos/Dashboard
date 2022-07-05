//saves filename from the server to the client's localStorage. This contains the data for all kpis requested by the user

//global variable to hold the xmlDocument
//var xmlstring = localStorage.getItem("xmlstring");
//var xml = new DOMParser().parseFromString(xmlstring, "text/xml");


function SaveXML(fileName) {
    var xml;

    $.ajax({
        url: fileName,
        dataType: "xml",
        cache: false,
        success: function (data) {
            xml = data;
            var xmlstring = new XMLSerializer().serializeToString(xml);
            localStorage.setItem("xmlstring", xmlstring);
        }
    });

}

function GetXMLData_All(kpiKey) {

   

    var key = 'row[KPIKey="' + kpiKey +'"]'
   
    
    //example of filtering the document for just those records that match a certain attribute value
    var allData = $(xml).find(key);

    return allData;
}

function LoadXMLData(kpiKey) {
    LoadXMLData_ColumnTitles(kpiKey);
    LoadXMLData_kpiTable(kpiKey);
    LoadXMLData_TitleBar(kpiKey)

}

function LoadXMLData_ColumnTitles(kpiKey) {

    /*
        Asssumes each column title is found in a row in the xml dataset with DataType="columnTitles"

    */

    var allData = GetXMLData_All(kpiKey);


    //filter for records in allData
    var dataType = 'row[DataType="columnTitles"]';

    var tableID = "#kpi";
        tableID += kpiKey;

    var RowDescription = '';
    var ID = '#';
    var colTitle = '';
    var hoverText = '';
    var cssClass = "columnTitle ";
    var hideOnMobile = "";


        var columnTitles = $(allData).filter(dataType);
        
       // console.log(columnCount);

            $(columnTitles).each(function () {
            colTitle = $(this).attr('RowDescription');
            hoverText = $(this).attr('string');
            ID = "#" + $(this).attr('ID');
            cssClass = "columnTitle " + $(this).attr('Class');
            hideOnMobile = $(this).attr('HideOnMobile');

            if (hideOnMobile == "1") {
                cssClass += " hidden-xs";
            }

            $(tableID +" thead tr").append( 
                $('<td>').text(colTitle).css("font-weight", "700").attr('title', hoverText).attr('class', cssClass).attr('id',ID)
            )
        })
}

function LoadXMLData_kpiTable(kpiKey) {

    var allData = GetXMLData_All(kpiKey);

    var dataType = 'row[DataType="tableData"]';
    var tableData = $(allData).filter(dataType);

    var ID = "#kpi";
    ID += kpiKey;

    var columnCount = $(ID + ' td').length;

    if(kpiKey == 65 || kpiKey == 63){
   console.log(ID + "column count:" + columnCount );
    }

    if (columnCount == 3) {

        $(tableData).each(function () {
            var attributeNodes = this.attributes;
            var rowID = $(this).attr('ID');
            var RowDescription = $(this).attr('RowDescription');

            var col1 = attributeNodes[4].value;// $(this).attr('A_CY_CM');
            var col2 = attributeNodes[5].value;
            var col3 = attributeNodes[6].value;

            $('<tr>').attr('id', rowID).append(
            $('<td>').text(RowDescription).attr('class', 'descriptionRow '),
            $('<td>').text(col1).attr('class', 'amount '),
            $('<td>').text(col2).attr('class', 'amount '),
            $('<td>').text(col3).attr('class', 'amount ')).appendTo(ID + " tbody");

        });
    }

    if (columnCount == 4) {

        $(tableData).each(function () {
            var attributeNodes = this.attributes;
            var rowID = $(this).attr('ID');
            var RowDescription = $(this).attr('RowDescription');

            var col1 = attributeNodes[4].value;// $(this).attr('A_CY_CM');
            var col2 = attributeNodes[5].value;
            var col3 = attributeNodes[6].value;
           

            $('<tr>').attr('id', rowID).append(
            $('<td>').text(RowDescription).attr('class', 'descriptionRow '),
            $('<td>').text(col1).attr('class', 'amount '),
            $('<td>').text(col2).attr('class', 'amount '),
            $('<td>').text(col3).attr('class', 'amount ')
        ).appendTo(ID + " tbody");

        });
    }

    if (columnCount == 5) {

        $(tableData).each(function () {
            var attributeNodes = this.attributes;
            var rowID = $(this).attr('ID');
            var RowDescription = $(this).attr('RowDescription');

            var col1 = attributeNodes[4].value;// $(this).attr('A_CY_CM');
            var col2 = attributeNodes[5].value;
            var col3 = attributeNodes[6].value;
            var col4 = attributeNodes[7].value;
            


            $('<tr>').attr('id', rowID).append(
            $('<td>').text(RowDescription).attr('class', 'descriptionRow '),
            $('<td>').text(col1).attr('class', 'amount '),
            $('<td>').text(col2).attr('class', 'amount '),
            $('<td>').text(col3).attr('class', 'amount '),
            $('<td>').text(col4).attr('class', 'amount ')
            
           
        ).appendTo(ID + " tbody");

        });
    }

    if(columnCount == 6){

        $(tableData).each(function () {
            var attributeNodes = this.attributes;
            var rowID = $(this).attr('ID');
            var RowDescription = $(this).attr('RowDescription');

                var col1 = attributeNodes[4].value;// $(this).attr('A_CY_CM');
                var col2 = attributeNodes[5].value;
                var col3 = attributeNodes[6].value;
                var col4 = attributeNodes[7].value;
                var col5 = attributeNodes[8].value;
                

            
                $('<tr>').attr('id',rowID).append(
                $('<td>').text(RowDescription).attr('class', 'descriptionRow '),
                $('<td>').text(col1).attr('class', 'amount '),
                $('<td>').text(col2).attr('class', 'amount '),
                $('<td>').text(col3).attr('class', 'amount '),
                $('<td>').text(col4).attr('class', 'amount '),
                $('<td>').text(col5).attr('class', 'amount ')
               

            ).appendTo(ID + " tbody");

        });
    }

    if (columnCount == 7) {

        $(tableData).each(function () {
            var attributeNodes = this.attributes;
            var rowID = $(this).attr('ID');
            var RowDescription = $(this).attr('RowDescription');

            var col1 = attributeNodes[4].value;// $(this).attr('A_CY_CM');
            var col2 = attributeNodes[5].value;
            var col3 = attributeNodes[6].value;
            var col4 = attributeNodes[7].value;
            var col5 = attributeNodes[8].value;
            var col6 = attributeNodes[9].value;
           


            $('<tr>').attr('id', rowID).append(
            $('<td>').text(RowDescription).attr('class', 'descriptionRow '),
            $('<td>').text(col1).attr('class', 'amount '),
            $('<td>').text(col2).attr('class', 'amount '),
            $('<td>').text(col3).attr('class', 'amount '),
            $('<td>').text(col4).attr('class', 'amount '),
            $('<td>').text(col5).attr('class', 'amount '),
            $('<td>').text(col6).attr('class', 'amount ')
           

        ).appendTo(ID + " tbody");

        });
    }

    if (columnCount == 8) {

        $(tableData).each(function () {
            var attributeNodes = this.attributes;
            var rowID = $(this).attr('ID');
            var RowDescription = $(this).attr('RowDescription');

            var col1 = attributeNodes[4].value;// $(this).attr('A_CY_CM');
            var col2 = attributeNodes[5].value;
            var col3 = attributeNodes[6].value;
            var col4 = attributeNodes[7].value;
            var col5 = attributeNodes[8].value;
            var col6 = attributeNodes[9].value;
            var col7 = attributeNodes[10].value;
           
            $('<tr>').attr('id', rowID).append(
            $('<td>').text(RowDescription).attr('class', 'descriptionRow '),
            $('<td>').text(col1).attr('class', 'amount '),
            $('<td>').text(col2).attr('class', 'amount '),
            $('<td>').text(col3).attr('class', 'amount '),
            $('<td>').text(col4).attr('class', 'amount '),
            $('<td>').text(col5).attr('class', 'amount '),
            $('<td>').text(col6).attr('class', 'amount '),
            $('<td>').text(col7).attr('class', 'amount ')

        ).appendTo(ID + " tbody");

        });
    }

    if (columnCount == 9) {

        $(tableData).each(function () {
            var attributeNodes = this.attributes;
            var rowID = $(this).attr('ID');
            var RowDescription = $(this).attr('RowDescription');

            var col1 = attributeNodes[4].value;// $(this).attr('A_CY_CM');
            var col2 = attributeNodes[5].value;
            var col3 = attributeNodes[6].value;
            var col4 = attributeNodes[7].value;
            var col5 = attributeNodes[8].value;
            var col6 = attributeNodes[9].value;
            var col7 = attributeNodes[10].value;
            var col8 = attributeNodes[11].value;

            $('<tr>').attr('id', rowID).append(
            $('<td>').text(RowDescription).attr('class', 'descriptionRow '),
            $('<td>').text(col1).attr('class', 'amount '),
            $('<td>').text(col2).attr('class', 'amount '),
            $('<td>').text(col3).attr('class', 'amount '),
            $('<td>').text(col4).attr('class', 'amount '),
            $('<td>').text(col5).attr('class', 'amount '),
            $('<td>').text(col6).attr('class', 'amount '),
            $('<td>').text(col7).attr('class', 'amount '),
            $('<td>').text(col8).attr('class', 'amount ')

        ).appendTo(ID + " tbody");

        });
    }

    if (columnCount == 10) {

        $(tableData).each(function () {
            var attributeNodes = this.attributes;
            var rowID = $(this).attr('ID');
            var RowDescription = $(this).attr('RowDescription');

            var col1 = attributeNodes[4].value;// $(this).attr('A_CY_CM');
            var col2 = attributeNodes[5].value;
            var col3 = attributeNodes[6].value;
            var col4 = attributeNodes[7].value;
            var col5 = attributeNodes[8].value;
            var col6 = attributeNodes[9].value;
            var col7 = attributeNodes[10].value;
            var col8 = attributeNodes[11].value;
            var col9 = attributeNodes[12].value;

            $('<tr>').attr('id', rowID).append(
            $('<td>').text(RowDescription).attr('class', 'descriptionRow '),
            $('<td>').text(col1).attr('class', 'amount '),
            $('<td>').text(col2).attr('class', 'amount '),
            $('<td>').text(col3).attr('class', 'amount '),
            $('<td>').text(col4).attr('class', 'amount '),
            $('<td>').text(col5).attr('class', 'amount '),
            $('<td>').text(col6).attr('class', 'amount '),
            $('<td>').text(col7).attr('class', 'amount '),
            $('<td>').text(col8).attr('class', 'amount '),
            $('<td>').text(col9).attr('class', 'amount ')

        ).appendTo(ID + " tbody");

        });
    }

    if (columnCount == 11) {

        $(tableData).each(function () {
            var attributeNodes = this.attributes;
            var rowID = $(this).attr('ID');
            var RowDescription = $(this).attr('RowDescription');

            var col1 = attributeNodes[4].value;// $(this).attr('A_CY_CM');
            var col2 = attributeNodes[5].value;
            var col3 = attributeNodes[6].value;
            var col4 = attributeNodes[7].value;
            var col5 = attributeNodes[8].value;
            var col6 = attributeNodes[9].value;
            var col7 = attributeNodes[10].value;
            var col8 = attributeNodes[11].value;
            var col9 = attributeNodes[12].value;
            var col10 = attributeNodes[13].value;

            $('<tr>').attr('id', rowID).append(
            $('<td>').text(RowDescription).attr('class', 'descriptionRow '),
            $('<td>').text(col1).attr('class', 'amount '),
            $('<td>').text(col2).attr('class', 'amount '),
            $('<td>').text(col3).attr('class', 'amount '),
            $('<td>').text(col4).attr('class', 'amount '),
            $('<td>').text(col5).attr('class', 'amount '),
            $('<td>').text(col6).attr('class', 'amount '),
            $('<td>').text(col7).attr('class', 'amount '),
            $('<td>').text(col8).attr('class', 'amount '),
            $('<td>').text(col9).attr('class', 'amount '),
            $('<td>').text(col10).attr('class', 'amount ')

        ).appendTo(ID + " tbody");

        });
    }

    if (columnCount == 12) {

        $(tableData).each(function () {
            var attributeNodes = this.attributes;
            var rowID = $(this).attr('ID');
            var RowDescription = $(this).attr('RowDescription');

            var col1 = attributeNodes[4].value;// $(this).attr('A_CY_CM');
            var col2 = attributeNodes[5].value;
            var col3 = attributeNodes[6].value;
            var col4 = attributeNodes[7].value;
            var col5 = attributeNodes[8].value;
            var col6 = attributeNodes[9].value;
            var col7 = attributeNodes[10].value;
            var col8 = attributeNodes[11].value;
            var col9 = attributeNodes[12].value;
            var col10 = attributeNodes[13].value;
            var col11 = attributeNodes[14].value;

            $('<tr>').attr('id', rowID).append(
            $('<td>').text(RowDescription).attr('class', 'descriptionRow '),
            $('<td>').text(col1).attr('class', 'amount '),
            $('<td>').text(col2).attr('class', 'amount '),
            $('<td>').text(col3).attr('class', 'amount '),
            $('<td>').text(col4).attr('class', 'amount '),
            $('<td>').text(col5).attr('class', 'amount '),
            $('<td>').text(col6).attr('class', 'amount '),
            $('<td>').text(col7).attr('class', 'amount '),
            $('<td>').text(col8).attr('class', 'amount '),
            $('<td>').text(col9).attr('class', 'amount '),
            $('<td>').text(col10).attr('class', 'amount '),
            $('<td>').text(col11).attr('class', 'amount ')

        ).appendTo(ID + " tbody");

        });
    }
}

function LoadXMLData_TitleBar(kpiKey) {

    var allData = GetXMLData_All(kpiKey);

    var filter = 'row[DataType="kpiTitleBar"]'

    //filter for title bar
    var allData = $(allData).filter(filter);

    //find the specific parameter
    
    var dataType = 'row[RowKey="100"]';

    var kpiTitle = $(allData).filter(dataType).attr("RowDescription");
    var kpiValue = $(allData).filter(dataType).attr("Col1");
    var kpiChange = $(allData).filter(dataType).attr("Col2");
    var kpiColor = $(allData).filter(dataType).attr("KPIColor");
    var kpiDetailDiv = $(allData).filter(dataType).attr("ID");
    var rounding = $(allData).filter(dataType).attr("Rounding");

    if (typeof kpiChange === "undefined") { kpiChange = '' }
    if (typeof rounding === "undefined") { rounding = '' }


    kpiValue += ' ' + rounding;

    //console.log(allData);

    // Adds the kpi header
    var html = '<!--KPI Title Bar-->'
    html += '<div class="portlet-heading bg-muted enabled collapsed" data-toggle="collapse" data-parent="#accordion1" href="#' + kpiDetailDiv + '">'
    html += '<h3 class="portlet-title">'
    html += '<a class="kpiTitle collapsed" data-toggle="collapse" data-parent="#accordion1" href="#' + kpiDetailDiv + '"><span class="glyphicon glyphicon-triangle-bottom dropdownArrow"></span><span class="padding">' + kpiTitle + '</span></a>'
    html += '</h3>'
    html += '<div class="portlet-widgets">'
    html += '<span id="ss_' + kpiDetailDiv + '" class="kpiValue">' + kpiValue + '</span>'
    html += '<span id="growth_' + kpiDetailDiv + '" class="kpiGrowth">' + kpiChange + '</span>'
    html += '<span id="light_' + kpiDetailDiv + '" class="fa fa-circle ' + kpiColor + '" style="font-size:16px;margin-left:5px;"></span>'
    html += '</div>'
    html += '<div class="clearfix"></div>'
    html += '</div>'

    $("#kpi" + kpiKey).before(html);


   

}


function GetXML_ParameterValue(parameterType) {

    var parameters = 'row[DataType="parameter"]'

    //filter for parameter values
    var allData = $(xml).find(parameters);

    //find the specific parameter
    var dataType = 'row[ParameterType="'+parameterType+'"]';
    var parameterValue = $(allData).filter(dataType).attr("ParameterValue");

    return parameterValue;

}

function GetXML_ParameterValueName(parameterType) {

    var parameters = 'row[DataType="parameter"]'

    //filter for parameter values
    var allData = $(xml).find(parameters);

    //find the specific parameter
    var dataType = 'row[ParameterType="' + parameterType + '"]';
    var parameterValueName = $(allData).filter(dataType).attr("ParameterValueName");

    return parameterValueName;

}
