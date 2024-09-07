var KeyPhraseChanged = function () {
   var _lstAdd = new Array();
    var _lstDelete = new Array();   
    $('.clsDivkeyphrases div').each(function () {
        if ($(this).attr('isDelete')== 1 && $(this).attr('isAdd') == 0) {
            _lstDelete.push($(this).text());
        }
        else if ($(this).attr('isDelete') == 0 && $(this).attr('isAdd') == 1) {
            _lstAdd.push($(this).text());
        }
    });
    this.lstDelete = _lstDelete;
    this.lstAdd = _lstAdd;
}
var keyphrase_List = null; var lastKeyPhraseID = null;


function ChangeKeyPhrase(type) {
    var isAdd = type == 0;
    if (!isAdd && lastKeyPhraseID == null) {
        ShowPopup("Law Search - Delete KeyPhrase", 'Please select a Keyphrase.', null);
        return;
    }
    var scontent = GetRowDataHTML(['<div style="text-align:left; color: red; display:none" class ="clsKeyphrase_Input_Error"></div>'], 'colspan="2"')
        + GetRowDataHTML(['<b>Keyphrase</b>', '<input class="clsKeyphrase_Input" type="textbox"  value="' + (isAdd ? '' : GetKeyphraseName()) + '" />'], '');

    if (!isAdd)
        scontent = 'Are you sure you want to delete "' + GetKeyphraseName()+'"?';
    var opt = [
        {
            name: 'OK', fClick: function () {

                if (isAdd && ($('.clsKeyphrase_Input').val() == null || $('.clsKeyphrase_Input').val() == "")) {
                    $('.clsKeyphrase_Input_Error').html('Please input Keyphrase'); $('.clsAddnewtype_Error').closest('tr').show();
                    return;
                } else if (isAdd) {
                    $('.clsKeyphrase_Input_Error').empty(); $('.clsAddnewtype_Error').closest('tr').hide();
                }
                ShowProcessing();                
            
                var skey = isAdd ? $('.clsKeyphrase_Input').val() : GetKeyphraseName();
                ClosePopup();
                PageMethods.Update_KeyPhrase(type, skey , function (data) {
                    HideProcessing();
                    if (type == 1) lastKeyPhraseID = null;
                    ViewAllKeyphrases();
                }, function () { HideProcessing(); });
            }
        }, { name: "Cancel", fClick: function () { ClosePopup(); } }
    ];
    ShowPopup("Law Search - Add KeyPhrase", scontent, opt);
}

function ViewAllKeyphrases() {
    $('.divlawKeyPhraseDetail').empty();
    ShowProcessing();

    PageMethods.GetAllKeyphrases_DB(function (data) {
        HideProcessing();
        var shtml = GetHTML_KeyPhraseContent(data);
        $('.clsConceptlist').html(shtml);
        if (lastKeyPhraseID != null)
            $('.clsKeyPhraseItem[KeyPhraseID=' + lastKeyPhraseID + ']').click();

    }, function () { HideProcessing(); });
}
function GetHTML_KeyPhraseContent(data) {
    if (data == null) return "";
    keyphrase_List = data;

    var shtml = '<div class="clsDivkeyphrases"><div> <input class="clsTextboxAutoCompletedKeyPhrase" type="textbox" onKeyUp ="Keyphrase_Filered();" /> <span style="text-decoration:none;">Total: ' + data.length.toString() + '<span></div>';

    for (var i = 0; i < data.length; i++) {
        shtml += '<div class="clsKeyPhraseItem"  onclick= "ViewKeyPhraseContent(' + data[i].ID + ', this);" KeyPhraseID ="' + data[i].ID + '"  >' + (i + 1).toString() + '. ' + data[i].Key + '</div>';
    }
    return shtml;
}

function GetKeyphraseName() {
    return $('.clsKeyPhraseSelected').text().substr($('.clsKeyPhraseSelected').text().indexOf('.') + 2);
}
function ViewKeyPhraseContent(kID, o) {
    $('.clsKeyPhraseSelected').removeClass('clsKeyPhraseSelected');
    $(o).addClass('clsKeyPhraseSelected');
    ShowProcessing();
    $('.divlawKeyPhraseDetail').empty();
    PageMethods.ViewKeyPhraseContent_Detail(kID, function (data) {
        HideProcessing();       
        lastKeyPhraseID = kID;
        var content = GetHTML_KeyPhraseRelateContent(data);
        $('.divlawKeyPhraseDetail').html(content);
    }, function () { HideProcessing(); });
 
  
}
function GetHTML_KeyPhraseRelateContent(data) {
    var stable = GetRowDataHTML(['<b>Chapter</b>', '<b>Artical</b>', '<b>Count</b>', '<b>Keyphrase</b>'], '');

    for (var i = 0; i < data.length; i++) {
        stable += GetRowDataHTML([data[i].ChapterName, data[i].ArticalName, data[i].NumCount, data[i].KeyPhrase], '');
    }
   
    stable = GetTableHTML(stable, 'clsTableKeyphrase');
    return stable;
}
function ViewKeyPhrases_Concept() { 
    if (lastConceptID == null) {
        ShowPopup('Law Search - Warning', 'Please select a Concept to View', null);
        return;
    }
    ShowProcessing();
    PageMethods.ViewDetailConcept_KeyPhrase(lastConceptID, function (data) {
        HideProcessing();
        ShowDetailConcept_KeyPhrases(data);
    }, function () { HideProcessing(); });
}
function ViewKeyPhrases() {
    var lstArticalID = new Array();
    if ($('[id$=cboDieu]').val() == -1) {
        $('[id$=cboDieu] option').each(function () {
            if ($(this).val() != -1)
                lstArticalID.push($(this).val());
        });
    }
    else
        lstArticalID.push($('[id$=cboDieu]').val());
    ShowProcessing();
    var LawID = $('[id$=cboLuat]').val();
    PageMethods.ViewDetailArtical_KeyPhrase(lstArticalID, LawID, function (data) {
        HideProcessing();
        if (data == null) return;
        var scontent = data.law == null ? '' : GetHTMLLawContent(data.law);
        ShowDetail_KeyPhrases(data.keys, scontent);
    }, function () {  HideProcessing(); });
}

function KeyPhraseChanged_Confirm(keyPhraseChanged) {

    if (keyPhraseChanged == null) return;
    if (keyPhraseChanged.lstAdd.length > 0 || keyPhraseChanged.lstDelete.length > 0) {
        //clsAddnewtype_Error
        var scontent = '';
        if (keyPhraseChanged.lstAdd.length>0)
            scontent += '<div><b>Thêm mới</b>: ' + keyPhraseChanged.lstAdd.join(' ') + ' </div>';
        if (keyPhraseChanged.lstDelete.length > 0)
            scontent += '<div><b>Xóa:</b> ' + keyPhraseChanged.lstDelete.join(' ') + ' </div>';
        var opt = [
            {
                name: 'Confirm', fClick: function () {
                    ClosePopup();
                    ShowProcessing();
                    PageMethods.KeyPhrase_Changed(keyPhraseChanged, function (data) {
                        HideProcessing();
                        ShowPopup('Law Search - KeyPhrases', 'Completed.', null);
                    }, function () { HideProcessing(); });
                },
            }, {
                name: "Cancel", fClick: function () { ClosePopup(); }
            }
        ];
      
        ShowPopup('Law Search - KeyPhrases', scontent, opt);
    }
}
function ShowDetailConcept_KeyPhrases(data) {
    var scontent = data == null ? '' : data.Content;
    ShowDetail_KeyPhrases(data.keys, scontent)
}

function ShowDetail_KeyPhrases(keys, scontent) {
   
    var skeyphrases = GetHTML_KeyPhrasesContent(keys);
    var adddeletekey = GetHTML_AddDeleteKeyPhrase();
 var sError = GetRowDataHTML(['<div style="text-align:left; color: red" class ="clsTableKeyphrase_Error"></div>'], 'colspan="2"');
   
    var stable = sError+ GetRowDataHTML([adddeletekey], 'colspan="2"')+ GetRowDataHTML([scontent, skeyphrases], '');
    stable = GetTableHTML(stable, 'clsTableKeyphrase');
    var opt = [
        {
            name: 'Update', fClick: function () {
               var keyPhraseChanged = new KeyPhraseChanged();
                ClosePopup();
                KeyPhraseChanged_Confirm(keyPhraseChanged);
            },      }, {    name: "Cancel", fClick: function () {     ClosePopup(); }
        }
    ];
    ShowPopup('Law Search - KeyPhrases', stable, opt);
}

function Keyphrase_Filered() {

    var v = $('.clsTextboxAutoCompletedKeyPhrase').val();
    v = v.replace(/ /g, '_');
    $('.clsDivkeyphrases div').each(function (idx) {
        if (idx == 0) return;
        if (v == '' || IsText1ContainText2( $(this).text(),v))
            $(this).show();
        else
            $(this).hide();
    });
}
function IsText1ContainText2(v1, v2) {
    var v11 = toLowerCaseNonAccentVietnamese(v1);
    var v22 = toLowerCaseNonAccentVietnamese(v2);
    var b = v11.indexOf(v22) >= 0;
    return b;
}

function GetHTML_KeyPhrasesContent(keys) {
    if (keys == null) return '';
    var shtml = '<div class="clsDivkeyphrases"><div> <input class="clsTextboxAutoCompletedKeyPhrase" type="textbox" onKeyUp ="Keyphrase_Filered();" /></div>';
   
    for (var i = 0; i < keys.length; i++) {
        shtml += GetKeyPhraseDiv(keys[i].Key,''); 
      //  shtml += keys[i].Key + ' ';
    }
    shtml += '</div>';
    return shtml;
}
function GetKeyPhraseDiv(key, appendInDiv) {

    return String.format('<div onclick="Keyphrase_Click(this);" {1} isDelete="0" >{0}</div>', key, appendInDiv);
}
//
function GetHTML_AddDeleteKeyPhrase() {

    var shtml = '<div class="clsDivkeyphrases_AddDelete">';
    shtml += 'Key Phrase: <input  class="clsDivkeyphrases_textbox" type ="textbox"> <input  type ="button" onclick="AddNewKeyPhrase()" value="Add" class="button150" />&nbsp; <input type ="button" value="Delete" onclick="DeleteKeyPhrase()" class="button150" />';
    shtml += '</div>';
    return shtml;
}

function Keyphrase_Click(o) {
    $('.clsDivkeyphrases_textbox').val($(o).text());
}
function GetFocuskey() {
    var newkey = $('.clsDivkeyphrases_textbox').val();
    if (newkey == '' || newkey == null) return '';
    newkey = newkey.replace(/ /g, "_");
    return newkey;
}
function AddNewKeyPhrase() {
    $('.clsTableKeyphrase_Error').empty();
    var newkey = GetFocuskey();
    if (newkey == '' || newkey == null) return;
    var f = false; var isreturn = false;
    $('.clsDivkeyphrases div').each(function () {
        if ($(this).text().toLowerCase() == newkey.toLowerCase()) {
            f = true;
            if ($(this).attr('isDelete') == 1) {
                $(this).attr('isDelete', 0);
                $(this).attr('isAdd', 1);
                $(this).css("color", "steelblue");
                isreturn = true;
                return;
            }

        }
    });
    if (isreturn) return;
    if (!f) {
        var snewkey = GetKeyPhraseDiv(newkey, 'class="clsDivkeyphrases_Newkey" isAdd="1"'); 
        $(snewkey).insertBefore($('.clsDivkeyphrases div').first());
    $('.clsDivkeyphrases_textbox').val('');
}
    else {
        $('.clsTableKeyphrase_Error').text('Keyphrase does exist.');
    }
}
function DeleteKeyPhrase() {
    $('.clsTableKeyphrase_Error').empty();
    var newkey = GetFocuskey();
    $('.clsDivkeyphrases div').each(function () {
        if ($(this).text().toLowerCase() == newkey.toLowerCase()) {
            if ($(this).attr('isAdd') == '1')
                $(this).remove();
            else {
                $(this).attr('isDelete', 1); $(this).attr('isAdd', 0);
                $(this).css("color", "red");
            }
        } 
    });
    $('.clsDivkeyphrases_textbox').val('');
}


