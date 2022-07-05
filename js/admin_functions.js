function addSpinner(target) {
    $(target).html("<h1 class='loading'>Loading...</h1><i class='fa fa-spinner fa-pulse fa-3x fa-fw loading-spinner'></i>");
}

function submitStatus() {
    $("#btnSubmit").html("Submitting...<i class='fa fa-spinner fa-pulse fa-1x fa-fw save-spinner'></i>");
    // check if the end date is set in the future. If not alerts the user and stop the function
    // creates Date instances for comparison
    var currentTime = Date.now();
    var startTime = new Date($("#start-date").val() + " " + $("#start-time").val())
    var endTime = new Date($("#end-date").val() + " " + $("#end-time").val())
    if (endTime < currentTime) {
        errorSubmit("The end date must be set in the future");
        return;
    } else if (endTime < startTime) {
        errorSubmit("The end date must be set after the start date");
        return;
    }
    // check that the user only picked one of the three targets. If not alerts the user and stop the function
    if (($("#isGlobal").val() == '1' && ($("#userID").val() != "" || $("#role").val() != "0")) || ($("#userID").val() != "" && $("#role").val() != "0")) {
        errorSubmit("You have to target the offline period either globally, for a role OR for a specific user.");
        return;
    } else if ($("#isGlobal").val() == "0" && $("#userID").val() == "" && $("#role").val() == "0") {
        errorSubmit("You must set a target");
        return;
    }
    // if conditions are met show the details to the user for last confirmation before post
    if (confirmStatus()) {
        addSpinner("#listStatus");
        $.post("editStatus.cshtml?", $("#frmEditStatus").serialize(), function () {
            $("#btnSubmit").text("Submit");
            $("#listStatus").load("ListStatus");
        });
    } else { $("#btnSubmit").text("Submit"); }
}

// if conditions are not met display a detailed error message to the user and prevent the post action
function errorSubmit(message) {
    alert(message);
    $("#btnSubmit").text("Submit");
}

// generates a confirm message with the offline period details before post
function confirmStatus() {
    var target = $("#userID").val() == '' ? $("#role option:selected").text() : "User ID " + $("#userID").val()
    if (target == 'Select a role') { target = 'Global'; }
    return confirm(
        "Please confirm the following details:\n\
            Target: " + target + "\n\
            From " + $("#start-date").val() + " " + $("#start-time").val()
        + " to " + $("#end-date").val() + " " + $("#end-time").val() + "\n\
            For " + $("#motive").val()
    )
}

//load the form with the offline period information that the user wish to update
function editStatus(recordKey) {
    // load the record key in the hidden input
    $("#RecordKey").val(recordKey);

    $("#form-title").text('Edit offline period');

    var isGlobal = $("#isGlobal-" + recordKey).length ? '1' : '0';
    $("#isGlobal option[value=" + isGlobal + "]").prop('selected', true)

    $("#UserKey-" + recordKey).length ? $("#userID").val($("#UserKey-" + recordKey).text().slice(8)) : $("#userID").val('')

    $("#GroupKey-" + recordKey).length ? $("#role").val($("#GroupKey-" + recordKey).data('groupkey')) : $("#role option[value='0']").prop('selected', true)

    // create Date instances to populate the date fields
    var startDate = new Date($("#StartDate-" + recordKey).text())
    var endDate = new Date($("#EndDate-" + recordKey).text())
    $("#start-date").val(startDate.toISOString().slice(0, 10));
    $("#start-time").val(startDate.toTimeString().slice(0, 5));
    $("#end-date").val(endDate.toISOString().slice(0, 10));
    $("#end-time").val(endDate.toTimeString().slice(0, 5));

    $("#motive").val($("#Motive-" + recordKey).text())
}

// update the targeted offline period by changing its ending date to the current date (effectively disabling it)
function disableStatus(recordKey) {
    editStatus(recordKey);
    var now = new Date()
    $("#end-date").val(now.toISOString().slice(0, 10));
    $("#end-time").val(now.toTimeString().slice(0, 5));
    if (confirm("Are you sure?")) {
        addSpinner("#listStatus");
        $.post("editStatus.cshtml?", $("#frmEditStatus").serialize(), function () {
            $("#btnSubmit").text("Submit");
            $("#listStatus").load("ListStatus")
        });
    } else { $("#btnSubmit").text("Submit"); }
    $("#RecordKey").val(0);
}