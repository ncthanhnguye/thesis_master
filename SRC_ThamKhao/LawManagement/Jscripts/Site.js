try {
    $(window).on("scroll resize", function (e) {
        Pagesize = getPageSize();
        _left = parseInt(Pagesize.width) / 2 - 175;
        _top = parseInt(Pagesize.height) / 2 - 60 + Pagesize.scrollTop;
        $("#panelUpdateProgress").css({ left: _left, top: _top });
        $(".BackgroundProcess").css({ top: Pagesize.scrollTop, left: Pagesize.scrollLeft });
        $(".popupover").css({ top: Pagesize.scrollTop, left: Pagesize.scrollLeft });
        $(".qtip-button").click();
    });

 //   preload(['../images/loading2.gif']);
} catch (e) {

}
function GetRequest() {
    var request = null;
    try {
        if (window.XMLHttpRequest) {
            //incase of IE7,FF, Opera and Safari browser
            request = new XMLHttpRequest();
        }
        else {
            //for old browser like IE 6.x and IE 5.x
            request = new ActiveXObject('MSXML2.XMLHTTP.3.0');
        }
    }
    catch (e) { }

    return request;
}
function OverlayBackground(isShow) {
    isShow ? $(".popupover").show() : $(".popupover").hide();
}
function HideShowProcessing(isShow) {
    var Pagesize = getPageSize();
    var _left = parseInt(Pagesize.width) / 2 - 175;
    var _top = parseInt(Pagesize.height) / 2 - 60 + Pagesize.scrollTop;
    $("#panelUpdateProgress").css({ left: _left, top: _top });
    isShow ? $("#panelUpdateProgress").show() : $("#panelUpdateProgress").hide();
    isShow ? $(".BackgroundPopup").show() : $(".BackgroundPopup").hide();
    OverlayBackground(isShow);
    $(".popupover").css({ top: Pagesize.scrollTop });
}
function GetPageRoot() {
    return window.location.href.substr(0, window.location.href.lastIndexOf('/'));
}
function RedirectPage(dest) {
    try {
        if (!dest.match('.aspx')) dest += '.aspx';
        window.location = GetPageRoot() + '/' + dest;
    } catch (e) {

    }
}
function HideProcessing() { HideShowProcessing(false); }
function ShowProcessing() { HideShowProcessing(true); }
function abortTask() {
    if (prm.get_isInAsyncPostBack())
        prm.abortPostBack();
    HideShowProcessing(false);
}
var prm = null;
$(document).ready(function () {
    try {
        prm = Sys.WebForms.PageRequestManager.getInstance();
        prm.add_initializeRequest(function (sender, args) {
            HideShowProcessing(true);
        });
        prm.add_endRequest(function (sender, args) {
            HideShowProcessing(false);
            try {
                var request = GetRequest();
                if (request != null && $('[id$=hdfPreSite]').val() != '') {
                    request.open("GET", $('[id$=hdfPreSite]').val() + '?keepalive=1', true);
                    request.send();
                }
            } catch (e) { }
        });
    } catch (e) { }
});
function open_in_new_tab(url) {
    if (url != '') {
        window.open(url, '_blank');
        window.focus();
    }
    else {
        GCTWindow.jWindow_Wrap('Cannot get data from server. Please try again later.', 'Law Search - Warning', null);
    }
}
function openUrl() {
    var url = document.getElementById("hidUrlLogo").value;
    open_in_new_tab(url);
}
function RedirectPage(dest) {
    try {
        if (!dest.match('.aspx')) dest += '.aspx';
        window.location = GetPageRoot() + '/' + dest;
    } catch (e) {

    }
}
function RedirectPageNoNewWindow(targetName) {
    if (targetName == 'BackLearnNewLanding') {
        $('[id$=btnHiddenBack]').click();
    }
    else if (targetName == 'LearnNewLanding') {
        $('[id$=btnHiddenHome]').click();
    }
    else if (targetName == 'LogOutLearn') {
        $('[id$=btnHiddenLogout]').click();
    }
}
function DoRedirect(surl) {
    window.location = surl;
}
function ShowBackground() {
    try {
        Pagesize = getPageSize();
        _top = parseInt(Pagesize.height) / 2 - 60 + Pagesize.scrollTop;
        $(".popupover").css({ top: Pagesize.scrollTop });
        $(".popupover").show();
    } catch (e) {

    }
}
function HideBackground() {
    $(".popupover").hide();
}

function Logout() {
    RedirectPageNoNewWindow('LogOutLearn');
}
function preload(arrayOfImages) {
    $(arrayOfImages).each(function () {
        $('<img/>')[0].src = this;
        // Alternatively you could use:
        // (new Image()).src = this;
    });
}
function ShowCenter(o) {
    try {
        var Pagesize = getPageSize();
        var height = $(o).height();
        var width = $(o).width();
        var _top = (Pagesize.height - height) / 2 + Pagesize.scrollTop;
        var _left = (Pagesize.width - width) / 2;
        $(o).css({ top: _top, left: _left }).show();
    } catch (e) {

    }
}
// Usage:

function OpenFilterByState() {
    try {
        var opt = [
                            { name: 'ok', fClick: function () {
                                //save
                                var arrSelected = new Array();
                                $('.clsListStateSelected div').each(function () {
                                    arrSelected.push($(this).attr('stateid'));
                                });
                                CustomizeAlertClose();
                                if (arrSelected.join(',') != $('[id$=hdfSelectedStateID]').val()) {
                                    $('[id$=hdfSelectedStateID]').val(arrSelected.join(','));
                                    $('[id$=btnFilter]').click();
                                }
                            }
                            },
                             { name: 'cancel', fClick: function () { CustomizeAlertClose(); } }
    ];
        var preSelectAgency = '';
        var iMaxState = $('[id$=hdfMaxState]').val();
        if (iMaxState == null || iMaxState == '' || isNaN(iMaxState) || parseInt(iMaxState) <= 0)
            iMaxState = 5;
        var isover = false;
        if ($('[id$=hdfSelectedStateID]').val() != '' && $('[id$=hdfSelectedStateID]').length != 0) {
            var arr = $('[id$=hdfSelectedStateID]').val().split(',');
            for (var i = 0; i < arr.length && i < iMaxState; i++) {
                preSelectAgency += '<div stateid="' + arr[i] + '">' + $('[id$=cboState] option[value=' + arr[i] + ']').text() + ' <a onclick="RemoveState(this);">x</a></div>';
            }
            isover = arr.length >= iMaxState;
        }
        var strhtml = '       <div class="clsBoxFilterState">'
                + '<div class="clsListStateSelected">' + preSelectAgency + '</div>'
                + '<div><input id="txtStateInput" type="text" placeholder="Enter State" class="clsTextBox"/></div>'
                + '<div>Enter up to ' + iMaxState + ' states that you want to filter results by.</div>'
                    + '</div>';

        GCTWindow.jWindow_Wrap_Style(strhtml, 'Filter Results by State(s)', opt);
        $('[id$=txtStateInput]').focus();
        if ($('[id$=cboState]').length == 1) {

            var arrState = GetArrayState();
            $('[id$=txtStateInput]').autocomplete({
                lookup: arrState,
                onSelect: function (so) {
                    if ($('.clsListStateSelected div[stateid=' + so.data + ']').length == 0)
                        $('.clsListStateSelected').append('<div stateid="' + so.data + '">' + so.value + ' <a onclick="RemoveState(this);">x</a></div>');
                    $('[id$=txtStateInput]').val('');
                    if ($('.clsListStateSelected div').length >= iMaxState)
                        $('[id$=txtStateInput]').attr("disabled", "disabled");
                },
                triggerSelectOnValidInput: true,
                autoSelectFirst: true,             
                lookupFilter: function (suggestion, query, queryLowerCase) { return suggestion.data.toLowerCase().indexOf(queryLowerCase) >= 0 || suggestion.value.toLowerCase().indexOf(queryLowerCase) >= 0; }
            });

        }

        if (isover)
            $('[id$=txtStateInput]').attr("disabled", "disabled");
    } catch (e) {

    }
}
function RemoveState(o) {
    $(o).parent().remove();
    $('[id$=txtStateInput]').removeAttr("disabled");
    $('[id$=txtStateInput]').focus();
}
function GetArrayState() {
    var arrState = new Array();
    $('[id$=cboState] option').each(function () {
        if ($('.clsListStateSelected div[stateid=' + $(this).val() + ']').length == 0)
            arrState.push(new StateInfo($(this).val(), $(this).text()));
    });
    return arrState;
}
var StateInfo = function (ID, Name) {
    this.data = ID;
    this.value = Name;
}