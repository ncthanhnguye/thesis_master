
var Clicker = null;
var curClaimsReferenceID = '';
var curAuthorizedPurpose = '';
var isNewCase = false;
var isRequireInput = false;
var isForceAudit = false;
var isAudited = false;
// comment bb
function SetAuditInfoClaimsReference(a, b, c, d, isNewCase, isAlready) {
    curClaimsReferenceID = a;
    isAudited = curClaimsReferenceID != '' || isAlready;
    curAuthorizedPurpose = b;
    isRequireInput = c;
    isForceAudit = d;
    this.isNewCase = isNewCase;
    if (curClaimsReferenceID == '')
        $('#divClaimsReferenceIDAudit').hide();
    else
        $('#divClaimsReferenceIDAudit').show();
}


$(document).ready(function () {
    $('[id$=btnNewCase]').click(function () {
        ShowPopupAuditInquiry(null);
    });
});
function setCaseNumberInfo(ClaimsReferenceID) {
    $('[id$=lblCaseAudit]').text(ClaimsReferenceID);

    if (ClaimsReferenceID != '')
        $('#divClaimsReferenceIDAudit').show();      
    else 
        $('#divClaimsReferenceIDAudit').hide();
}

function ShowPopupAuditInquiry(clicker) {
  
    if (isForceAudit) {
        
        if (clicker != null && isAudited) {
            if (!isNewCase)
                isAudited = false;
            return true;
        }
        Clicker = clicker;
       
        var opt = [
                            { name: 'continue', fClick: ContinueAudit_Click }
        ];

        if (!isRequireInput)
        opt.push({ name: 'Skip', fClick: ButtonSkipAudit_Click });
        else
            opt.push({ name: 'Clear', fClick: ClearAudit_Click });
                    
        var strhtml = '';
    var sPls = '', sTar = '';
    if (isRequireInput) {
        sPls = '<p class="text_10bold" style="font-weight:normal;text-align:center;color:#000">Please complete the following information before proceeding.</p>';
        sTar = '<span style="color:#e81515">* </span>';
    }
    strhtml += '<div style="text-align:left">'
                  + sPls
                  + '<div style="text-align:center;margin-top:15px;">' + sTar + '<b>Reference ID: </b>'

                  + '<input type="text" id="txtClaimsReferenceID" style="width: 208px; height: 22px" value=""  maxlength="50"/></div>'
                  + '<div style="padding-top: 10px;color:#e81515" class="lblErrorAudit"></div></div>';
    GCTWindow.jWindow_Wrap(strhtml, 'Law Search - Audit Inquiry', opt);
    $('#txtClaimsReferenceID').val(curClaimsReferenceID);
    $('#btnCloseCustomPanel').click(function () { if (!isRequireInput) SkipAudit_Click(); });
        $('#txtClaimsReferenceID').focus();
    $('#txtClaimsReferenceID').keyup(function (e) {
        if (e.keyCode == 13) {
                ContinueAudit_Click();
            }
        });

        return false;
    }
    else
        return true;
}
function ButtonSkipAudit_Click() {
    curClaimsReferenceID = '';
    if (isNewCase)
        setCaseNumberInfo('');
    SkipAudit_Click();
}
function SkipAudit_Click() {
    GCTWindow.closeMessage();
    isAudited = true;
    var surl = document.location.href;
    var rootURL = surl.substr(0, surl.lastIndexOf("/"));
    surl = rootURL + "/AuditInfo.aspx/SetAuditInfoSession";
    $.ajax({
        url: surl,
        type: "POST",
        cache: false,
        data: "{'curClaimsReferenceID':'" + curClaimsReferenceID + "', 'curAuthorizedPurpose':''}",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            if (response == 'success' || response.d == 'success') {
                if (Clicker != null)
                    $(Clicker).click();
            }
            else if (response == 'timeout' || response.d == 'timeout')
                window.location = rootURL + 'Home.aspx';
        }
    });
}

function ContinueAudit_Click() {
    var txtClaimsReferenceID = $('#txtClaimsReferenceID').val();

    if (txtClaimsReferenceID == '') {
        var mess = isRequireInput ? 'Please input the Reference ID to continue.' : 'Please input a Reference ID or select option to Skip.';
        $('.lblErrorAudit').html(mess);
        return;
    } else if (txtClaimsReferenceID.length < 3) {
        $('.lblErrorAudit').html('Please enter at least four (3) characters for the Reference ID entry');
        return;
    } else if (txtClaimsReferenceID.length > 50) {
        $('.lblErrorAudit').html(' Please revise your Reference ID. Maximum size = 50 characters.');
        return;
    } else if (!validText(txtClaimsReferenceID)) {
        $('.lblErrorAudit').html('<center><p>Reference ID cannot contain any of these characters:</p><p> \\ / | @ # $ % ^ * ~ < > & ; \' "</p></center>');
        return;
    }



    //gan session
    var surl = document.location.href;
    var rootURL = surl.substr(0, surl.lastIndexOf("/"));
    surl = rootURL + "/AuditInfo.aspx/SetAuditInfoSession";
    $.ajax({
        url: surl,
        type: "POST",
        cache: false,
        data: "{'curClaimsReferenceID':'" + txtClaimsReferenceID + "', 'curAuthorizedPurpose':''}",
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (response) {
            isAudited = true;
            if (response == 'success' || response.d == 'success') {
                curClaimsReferenceID = txtClaimsReferenceID;
              
                if (isNewCase)
                    setCaseNumberInfo(curClaimsReferenceID);

                if (Clicker != null) {
                    //goi tiep ham search
                    $(Clicker).click();
                }
            }
            else if (response == 'timeout' || response.d == 'timeout') {
                window.location = rootURL + '/Home.aspx';
            }
        },
        error: function (error) {
            GCTWindow.jWindow_Wrap('Cannot get data from server. Please try again later.', 'Law Search - Warning', null);
        }
    });

    GCTWindow.closeMessage();
}
function CancelAudit_Click() {
    GCTWindow.closeMessage();
}
function ClearAudit_Click() {
    $('#txtClaimsReferenceID').val('');
    $('#txtClaimsReferenceID').focus();
}

function validText(value) {
    var chaos = new Array("~", "@", "#", "$", "%", "^", "&", "*", ";", "/", "\\", "|", "<", ">", "'", "\"");
    var sum = chaos.length;   
    for (var i in chaos) { if (!Array.prototype[i]) { sum += value.lastIndexOf(chaos[i]) } }
    if (sum) {
        return false;
    }
    return true;
}