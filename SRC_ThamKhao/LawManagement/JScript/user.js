var UserChanged = function () {
   var _lstAdd = new Array();
    var _lstDelete = new Array();   
    $('.clsDivUsers div').each(function () {
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
var User_List = null; var lastUserID = null;


function ChangeUser(type) {
    var isAdd = type == 0;
    if (type == 1 && lastUserID == null) {
        ShowPopup("Law Search - Delete User", 'Please select a User.', null);
        return;
    }
    var scontent = GetRowDataHTML(['<div style="text-align:left; color: red; display:none" class ="clsUser_Input_Error"></div>'], 'colspan="2"');
    if (type==0)
        scontent+= GetRowDataHTML(['<b>Username:</b>', '<input class="clsUser_Input" type="textbox"  value="' + (isAdd ? '' : GetUserName()) + '" />'], '')
        + GetRowDataHTML(['<b>Password:</b>', '<input class="clsUser_Pass" type="Password"  value="" />'], '');
    else if (type == 2)
        scontent += GetRowDataHTML(['<b>Enter New Password:</b>', '<input class="clsUser_Pass" type="Password"   value="" />'], '')
            + GetRowDataHTML(['<b>Re-enter New Password:</b>', '<input class="clsUser_Pass2" type="Password"  value="" />'], '');
    if (type == 1)
        scontent = 'Are you sure you want to delete "' + GetUserName()+'"?';
    var opt = [
        {
            name: 'OK', fClick: function () {

                if (isAdd && ($('.clsUser_Input').val() == null || $('.clsUser_Input').val() == "" || $('.clsUser_Pass').val() == "")) {
                    $('.clsUser_Input_Error').html('Please input User & password'); $('.clsAddnewtype_Error').closest('tr').show();
                    return;
                } else if (type == 2 && ($('.clsUser_Pass').val() == null || $('.clsUser_Pass').val() != $('.clsUser_Pass2').val())) {
                    $('.clsUser_Input_Error').html('Passwords does not match'); $('.clsAddnewtype_Error').closest('tr').show();
                    return;
                } else if (isAdd) {
                    $('.clsUser_Input_Error').empty(); $('.clsAddnewtype_Error').closest('tr').hide();
                }
                ShowProcessing();                
            
                var skey = isAdd ? $('.clsUser_Input').val() : GetUserName();
                var pass = $('.clsUser_Pass').length == 0 ? '' : $('.clsUser_Pass').val();
                ClosePopup();
                PageMethods.Update_User(type, skey, pass, function (data) {
                    HideProcessing();
                    if (type == 1) lastUserID = null;
                    ViewAllUsers();
                }, function () { HideProcessing(); });
            }
        }, { name: "Cancel", fClick: function () { ClosePopup(); } }
    ];
    ShowPopup("Law Search - Add User", scontent, opt);
}
var lstAllUsers = null;
function ViewAllUsers() {
    $('.divlawUserDetail').empty();
    ShowProcessing();

    PageMethods.GetAllUsers_DB(function (data) {
        HideProcessing();
        var shtml = GetHTML_UserContent(data);
      
        $('.clsConceptlist').html(shtml);
        if (lastUserID != null)
            $('.clsUserItem[UserID=' + lastUserID + ']').click();

    }, function () { HideProcessing(); });
}
function GetHTML_UserContent(data) {
    if (data == null) return "";
    User_List = data;

    var shtml = '<div class="clsDivUsers"><div> <input class="clsTextboxAutoCompletedUser" type="textbox" onKeyUp ="User_Filered();" /> <span style="text-decoration:none;">Total: ' + data.length.toString() + '<span></div>';
    lstAllUsers = data;
    for (var i = 0; i < data.length; i++) {
        shtml += '<div class="clsUserItem"  onclick= "ViewUserContent(' + data[i].ID + ', this);" UserID ="' + data[i].ID + '"  >' + (i + 1).toString() + '. ' + data[i].username + '</div>';
    }
    return shtml;
}

function GetUserName() {
    return $('.clsUserSelected').text().substr($('.clsUserSelected').text().indexOf('.') + 2);
}
function ViewUserContent(kID, o) {
    $('.clsUserSelected').removeClass('clsUserSelected');
    $(o).addClass('clsUserSelected');
  
    $('.divlawUserDetail').empty();

    var shtml = '';
    for (var i = 0; i < lstAllUsers.length; i++) {
        if (kID == lstAllUsers[i].ID) {
            shtml += '<div class="clsUserItem"   >Username: ' + lstAllUsers[i].username + '</div>';
            lastUserID = kID;
            break;
        }
    }
    $('.divlawUserDetail').html(shtml);

  
}
function GetHTML_UserRelateContent(data) {
    var stable = GetRowDataHTML(['<b>Chapter</b>', '<b>Artical</b>', '<b>Count</b>', '<b>User</b>'], '');

    for (var i = 0; i < data.length; i++) {
        stable += GetRowDataHTML([data[i].ChapterName, data[i].ArticalName, data[i].NumCount, data[i].User], '');
    }
   
    stable = GetTableHTML(stable, 'clsTableUser');
    return stable;
}
function ViewUsers_Concept() { 
    if (lastConceptID == null) {
        ShowPopup('Law Search - Warning', 'Please select a Concept to View', null);
        return;
    }
    ShowProcessing();
    PageMethods.ViewDetailConcept_User(lastConceptID, function (data) {
        HideProcessing();
        ShowDetailConcept_Users(data);
    }, function () { HideProcessing(); });
}
function ViewUsers() {
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
    PageMethods.ViewDetailArtical_User(lstArticalID, LawID, function (data) {
        HideProcessing();
        if (data == null) return;
        var scontent = data.law == null ? '' : GetHTMLLawContent(data.law);
        ShowDetail_Users(data.keys, scontent);
    }, function () {  HideProcessing(); });
}

function UserChanged_Confirm(UserChanged) {

    if (UserChanged == null) return;
    if (UserChanged.lstAdd.length > 0 || UserChanged.lstDelete.length > 0) {
        //clsAddnewtype_Error
        var scontent = '';
        if (UserChanged.lstAdd.length>0)
            scontent += '<div><b>Thêm mới</b>: ' + UserChanged.lstAdd.join(' ') + ' </div>';
        if (UserChanged.lstDelete.length > 0)
            scontent += '<div><b>Xóa:</b> ' + UserChanged.lstDelete.join(' ') + ' </div>';
        var opt = [
            {
                name: 'Confirm', fClick: function () {
                    ClosePopup();
                    ShowProcessing();
                    PageMethods.User_Changed(UserChanged, function (data) {
                        HideProcessing();
                        ShowPopup('Law Search - Users', 'Completed.', null);
                    }, function () { HideProcessing(); });
                },
            }, {
                name: "Cancel", fClick: function () { ClosePopup(); }
            }
        ];
      
        ShowPopup('Law Search - Users', scontent, opt);
    }
}
function ShowDetailConcept_Users(data) {
    var scontent = data == null ? '' : data.Content;
    ShowDetail_Users(data.keys, scontent)
}

function ShowDetail_Users(keys, scontent) {
   
    var sUsers = GetHTML_UsersContent(keys);
    var adddeletekey = GetHTML_AddDeleteUser();
 var sError = GetRowDataHTML(['<div style="text-align:left; color: red" class ="clsTableUser_Error"></div>'], 'colspan="2"');
   
    var stable = sError+ GetRowDataHTML([adddeletekey], 'colspan="2"')+ GetRowDataHTML([scontent, sUsers], '');
    stable = GetTableHTML(stable, 'clsTableUser');
    var opt = [
        {
            name: 'Update', fClick: function () {
               var UserChanged = new UserChanged();
                ClosePopup();
                UserChanged_Confirm(UserChanged);
            },      }, {    name: "Cancel", fClick: function () {     ClosePopup(); }
        }
    ];
    ShowPopup('Law Search - Users', stable, opt);
}

function User_Filered() {

    var v = $('.clsTextboxAutoCompletedUser').val();
    v = v.replace(/ /g, '_');
    $('.clsDivUsers div').each(function (idx) {
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

function GetHTML_UsersContent(keys) {
    if (keys == null) return '';
    var shtml = '<div class="clsDivUsers"><div> <input class="clsTextboxAutoCompletedUser" type="textbox" onKeyUp ="User_Filered();" /></div>';
   
    for (var i = 0; i < keys.length; i++) {
        shtml += GetUserDiv(keys[i].Key,''); 
      //  shtml += keys[i].Key + ' ';
    }
    shtml += '</div>';
    return shtml;
}
function GetUserDiv(key, appendInDiv) {

    return String.format('<div onclick="User_Click(this);" {1} isDelete="0" >{0}</div>', key, appendInDiv);
}
//
function GetHTML_AddDeleteUser() {

    var shtml = '<div class="clsDivUsers_AddDelete">';
    shtml += 'Key Phrase: <input  class="clsDivUsers_textbox" type ="textbox"> <input  type ="button" onclick="AddNewUser()" value="Add" class="button150" />&nbsp; <input type ="button" value="Delete" onclick="DeleteUser()" class="button150" />';
    shtml += '</div>';
    return shtml;
}

function User_Click(o) {
    $('.clsDivUsers_textbox').val($(o).text());
}
function GetFocuskey() {
    var newkey = $('.clsDivUsers_textbox').val();
    if (newkey == '' || newkey == null) return '';
    newkey = newkey.replace(/ /g, "_");
    return newkey;
}
function AddNewUser() {
    $('.clsTableUser_Error').empty();
    var newkey = GetFocuskey();
    if (newkey == '' || newkey == null) return;
    var f = false; var isreturn = false;
    $('.clsDivUsers div').each(function () {
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
        var snewkey = GetUserDiv(newkey, 'class="clsDivUsers_Newkey" isAdd="1"'); 
        $(snewkey).insertBefore($('.clsDivUsers div').first());
    $('.clsDivUsers_textbox').val('');
}
    else {
        $('.clsTableUser_Error').text('User does exist.');
    }
}
function DeleteUser() {
    $('.clsTableUser_Error').empty();
    var newkey = GetFocuskey();
    $('.clsDivUsers div').each(function () {
        if ($(this).text().toLowerCase() == newkey.toLowerCase()) {
            if ($(this).attr('isAdd') == '1')
                $(this).remove();
            else {
                $(this).attr('isDelete', 1); $(this).attr('isAdd', 0);
                $(this).css("color", "red");
            }
        } 
    });
    $('.clsDivUsers_textbox').val('');
}


