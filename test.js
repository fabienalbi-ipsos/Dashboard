function GetTree(isInit) {
    var waiting = false;
    if ($("#treeContent").html() !== "") {
        $("#treeModal").modal('toggle');
    }
    else { //the tree content is not loaded yet
        $('#tree-link').on('click', function () { // if the user click on the tree link before it is loaded a loading spinned is displayed
            waiting = true;
            $("#btnScope").html('<i class="fa fa-circle-o-notch fa-spin pull-left" style="font-size:16px;color:black;margin-right:5px;margin-top:2px;"></i>Loading...');
        });
    }

    if (isInit) {
        $.ajax({
            url: "content/controls/trees.cshtml",
            cache: false,
            dataType: "html",

            success: function (data) {
                $("#treeContent").html(data).promise().done(function () {
                    if (waiting) {
                        $("#treeModal").modal('toggle');
                    }
                    $("#btnScope").html("@defaultScope");

                    $('#tree-link').on('click', function () { // the tree is now loaded so we no longer display a spinner on click
                        $("#btnScope").html("@defaultScope");
                    });//close treelink
                });//close treecontent
            }//close success

        }); //close ajax
    }//close isInit
}//close the function

$(document).ready(function () {
    //Log the time it took to load the page in the PageLog table
    var PageLog = "../content/controls/PageLog?parameterType=1&PageName=Index";
    $.post(PageLog, function () { });
    var PageLog = "../content/controls/PageLog?parameterType=0&PageName=Ajax";
    $.post(PageLog, function () { });

    var False = false;
    var True = true;

    //disable headlines if not admin
    $('#portlet-kpiSummary').slideDown(1500, function () { $('#portlet-kpiSummary').addClass("in").removeAttr('style'); });

    //hide future periods from parameters
    if (parseInt($('#selectedYear').text()) == new Date().getFullYear()) {
        $('#periodsDropDown > li > a').each(function () {
            if ($(this).attr('id') - 1 > new Date().getMonth()) {
                $(this).css({ 'display': 'none' });
            }
        });
    }

    //check what scope the user has checked
    var selectedScope = $("input:checked").text();
    $("#selected").text(selectedScope);




    $('#landing h2').each(function () {
        if ($(this).text() == "N/A") {
            $(this).parent('div').addClass("disabled");
        }
    });

    GetTree(true);

    var PageLog = "../content/controls/PageLog?parameterType=1&PageName=Ajax";
    $.post(PageLog, function () { });

    //datatables
    function format(d) {
        // `d` is the original data object for the row
        return '<table cellpadding="5" cellspacing="0" border="0" style="padding-left:50px; table-layout: fixed; width: 600px;">' +
            '<tr>' +
            '<td><span style="font-weight: bold">Proposal ID:</span></td>' +
            '<td>' + d[1] + '</td>' +
            '</tr>' +
            '<tr>' +
            '<td>Create Date:</td>' +
            '<td>' + d[5] + '</td>' +
            '</tr>' +
            '<tr>' +
            '<td>Proposal Owner:</td>' +
            '<td>' + d[9] + '</td>' +
            '</tr>' +
            '<tr>' +
            '<td style = "word-wrap:break-word">Comment:</td>' +
            '<td>' + d[10] + '</td>' +
            '</tr>' +
            '<tr>' +
            '<td>Extra info:</td>' +
            '<td>Journal entries here...</td>' +
            '</tr>' +
            '</table>';
    }


    var date = new Date();
    var table = $('#main-table').DataTable({
        "lengthMenu": [[100, 200, 500, -1], [100, 200, 500, "All"]],
        fixedHeader: true,
        paging: true,
        "lengthChange": true,
        "processing": true,

        "oSearch": {
            "bSmart": false,
            "bRegex": true,
            "sSearch": ""
        },
        "order": [[5, "desc"]],

        dom: 'Blrftip',
        buttons: [
            'copyHtml5',
            {
                extend: 'excelHtml5',
                filename: 'Pipeline_' + date.toLocaleDateString()
            },
        ]
    });



    // Add event listener for opening and closing details
    $('#main-table tbody').on('click', 'td.fa-plus-circle', function () {
        var tr = $(this).closest('tr');
        var row = table.row(tr);

        if (row.child.isShown()) {
            // This row is already open - close it
            row.child.hide();
            tr.removeClass('shown');
        }
        else {
            // Open this row
            row.child(format(row.data())).show();
            tr.addClass('shown');
        }
    });


    $('#main-table').show('slow');



    $(window).on('resize', function () {
        resizeTable();
    });



    $('.widget-style-1').click(function () {
        var ActivityName = $(this).find('a').text().replace('&', 'and');
        $.post("/content/controls/UserActivityLog_Set.cshtml?ActivityType=Headlines&ActivityName=" + ActivityName);
    });



    $('div.portlet-heading.bg-muted.enabled').one('click', function () {
        var ActivityName = $(this).find('span.padding').text().replace('&', 'and');
        $.post("/content/controls/UserActivityLog_Set.cshtml?ActivityType=KPI&ActivityName=" + ActivityName);
    });

    $('div.portlet-section').one('click', function () {
        var ActivityName = $(this).find('span.padding').text().replace('&', 'and');
        $.post("/content/controls/UserActivityLog_Set.cshtml?ActivityType=Section&ActivityName=" + ActivityName);
    });

    $('.glyphicon-question-sign').click(function () {
        var ActivityName = $(this).attr('title').replace('&', 'and');
        $.post("/content/controls/UserActivityLog_Set.cshtml?ActivityType=Help&ActivityName=" + ActivityName);
    });
    $('.navbar-brand').click(function () { $.post("/content/controls/UserActivityLog_Set.cshtml?ActivityType=Navigation&ActivityName=Pipeline Reload"); });
    $('#expand-all-button').click(function () { $.post("/content/controls/UserActivityLog_Set.cshtml?ActivityType=Navigation&ActivityName=Expand All"); });
    $('#collapse-all-button').click(function () { $.post("/content/controls/UserActivityLog_Set.cshtml?ActivityType=Navigation&ActivityName=Collapse All"); });

}); //end document.ready
