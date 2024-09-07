// JScript File

function GetButtonText(name) {
    switch (name.toLowerCase()) {
        case "yes":
            return "Yes";
            break;
        case "no":
            return "No";
            break;
        case "ok":
            return "OK";
            break;
        case "cancel":
            return "Cancel";
            break;
        case "continue":
            return "Continue";
            break;
        case "getxls":
            return "Get XLS";
            break;
        case "getpdf":
            return "Get PDF";
            break;
        case "execute":
            return "Execute";
            break;
        case "new":
            return "New";
            break;
        case "delete":
            return "Delete";
            break;
        case "edit":
            return "Edit";
            break;
        case "close":
            return "Close";
            break;
        case "save":
            return "Save";
            break;
        case "swap":
            return "Swap";
            break;
        case "outputreport":
            return "Output Report";
            break;
        case "configure":
            return "Configure";
            break;
        case "schedule":
            return "Schedule";
            break;
        case "refresh":
            return "Refresh";
        case "view":
            return "View";
        case "clear":
            return "Clear";
        case "finish":
            return "Finish";
        case "add":
            return "Add";
            break;
    }
    return name;
}
$(document).ready(function () {
    MouseEventButton('');
    try {
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_endRequest(function (sender, args) {
            MouseEventButton('');
        });
    } catch (e) {

    }

});
//Thinh
function MouseEventButton(id) {
    var obj = "";
    if (id != "")
        obj = "#" + id;
    else
        obj = "body";
    var arr = new Array();
    arr.push(".button100"); arr.push(".button150");
    //for (var i = 0; i < arr.length; i++)
    //    $(obj).find(arr[i]).each(function () {
    //        if ($(this).is(":enabled")) {
    //            $(this)
    //                .mouseover(function () { $(this).css("background-color", "#88A1C2"); })
    //                .mouseout(function () { $(this).css("background-color", "#ffffff"); })
    //                .css("background-color", "#ffffff");
    //        }
    //        else
    //            $(this).css("background-color", "#d4d4d4");
    //    });
}
//Global functions
function formatWithComma(number) {

    if (number == 0)
        return "0";
    var formattedNumberString = (number % 1000).toString();
    if (number >= 1000) {
        if (formattedNumberString == "0")
            formattedNumberString = "000";
        if (formattedNumberString.length == 1)
            formattedNumberString = "00" + formattedNumberString;
        if (formattedNumberString.length == 2)
            formattedNumberString = "0" + formattedNumberString;
    }
    var x = parseInt(number / 1000);
    var xString;
    while (x > 0) {
        if (x % 1000 == 0)
            formattedNumberString = '000,' + formattedNumberString;
        else {
            xString = (x % 1000).toString();

            if (x >= 1000) {
                if (xString.length == 1)
                    xString = "00" + xString;
                else if (xString.length == 2)
                    xString = "0" + xString;
            }
            formattedNumberString = xString + ',' + formattedNumberString;
        }

        x = parseInt(x / 1000);
    }
    return formattedNumberString;
}