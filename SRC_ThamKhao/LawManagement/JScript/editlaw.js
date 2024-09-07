var SelectedLawItemType = function () {
    this.type = $('.clsAddnewtype').val();
    this.chapterID = this.type == 0 ? 0 : $('.clsAddnewsubtype_Chapter').val();
    this.chapteritemID = this.type < 2 ? 0 : $('.clsAddnewsubtype_ChapterItem').val();
    this.LawID = $('[id$=cboLuat]').val();
    this.isAuto = $('.clsAddnewcheckboxExtractAuto input:checkbox').is(':checked');
    this.Name = !this.isAuto ? $('.clsAddnewtype_manual_Name').val() : '';
    this.Title = !this.isAuto ? $('.clsAddnewtype_manual_Title').val() : '';
    var nn = $('.clsAddnewtype_manual_Number').val();
    this.Number = !this.isAuto ? (nn == '' || nn == null || isNaN(nn) ? -1 : parseInt(nn)) : -1;
    this.ArticalID = this.type == 3?  $('.clsAddnewsubtype_Artical').val(): 0;
    this.content = !this.isAuto ? $('.clsAddnewtype_manual_Content').val() : $('.clsAddnewtype_content').val();
    this.ClauseID = 0;
}

function GetContentHTMLEdit(type, parentID, ID, childID) {
    var data = GetMylaw();
    var scontent = '';
    var shtml = '';
    var sformatName = GetRowDataHTML(['<b>{0}</b>', '<input class="{1}" type="textbox"  value="{2}" />'], '');
    var sformatContent = GetRowDataHTML(['<b>{0}</b>', '<textarea class="{1}" type="textbox"  value="{2}" />'], '');
    for (var i = 0; i < data.lstChapters.length; i++) {
        var chapter = data.lstChapters[i];
        if (type !=3 && parentID != chapter.ID)
            continue;
        if (type == 0) {
            shtml += String.format(sformatName, 'Tên:', 'clsAddnewtype_manual_Name', chapter.Name);
            shtml += String.format(sformatName, 'Tiêu đề:', 'clsAddnewtype_manual_Title', chapter.Title);
            shtml += String.format(sformatName, 'Số:', 'clsAddnewtype_manual_Number', chapter.Number);
            return shtml;          
        }
        for (var j = 0; chapter.lstChapterItem != null && j < chapter.lstChapterItem.length; j++) {
            var chapterItem = chapter.lstChapterItem[j];
            if (type == 1 && ID != chapterItem.ID)
                continue;
            if (type == 1) {
                //set html
      
                shtml += String.format(sformatName, 'Tên:', 'clsAddnewtype_manual_Name', chapterItem.Name);
                shtml += String.format(sformatName, 'Tiêu đề:', 'clsAddnewtype_manual_Title', chapterItem.Title);
                shtml += String.format(sformatName, 'Số:', 'clsAddnewtype_manual_Number', chapterItem.Number);
                return shtml;        
            }
      
            for (var k = 0; chapterItem.lstArtical != null && k < chapterItem.lstArtical.length; k++) {
                var artical = chapterItem.lstArtical[k];
                if (type == 2 && childID != artical.ID)
                    continue;
                if (type == 2) {
                    //set html
                    shtml += String.format(sformatName, 'Tên:', 'clsAddnewtype_manual_Name', artical.Name);
                    shtml += String.format(sformatName, 'Tiêu đề:', 'clsAddnewtype_manual_Title', artical.Title);
                    shtml += String.format(sformatName, 'Số:', 'clsAddnewtype_manual_Number', artical.Number);
                    return shtml;        
                }

                for (var l = 0; artical.lstClause != null && l < artical.lstClause.length; l++) {
                    var clause = artical.lstClause[l];
                    if (type == 3 && childID != clause.ID)
                        continue;                  
                    shtml += String.format(sformatName, 'Tiêu đề:', 'clsAddnewtype_manual_Title', clause.Title);
                    shtml += String.format(sformatName, 'Số:', 'clsAddnewtype_manual_Number', clause.Number);
                    return shtml;        
                }
            }
        }

    }
}
function DeleteLaw(type, parentID, ID) {
    var opt = [
        {
            name: 'Delete', fClick: function () {
                var LawID = $('[id$=cboLuat]').val();
                ShowProcessing();
                //clsAddnewtype_Error
                PageMethods.DeleteLaw(LawID, type, parentID, ID, function (data) {
                    HideProcessing(); ClosePopup();
                    if (data == 1)
                        ReloadMylaw();
                    else
                        ShowPopup("Law Search - Waring", "Item is not empty.", null);
                    
                }, function () { HideProcessing(); });
            }
        }, {
            name: "Cancel", fClick: function () { ClosePopup(); }
        }
    ];
    ShowPopup("Law Search - Waring", "Are you sure you want to delete this Item ?", opt);
}

function checkboxExtractAuto_Changed() {
    if ($('.clsAddnewcheckboxExtractAuto input:checkbox').is(':checked')) {
        $('.clsAddnewtype_auto').show();
        $('.clsAddnewtype_manual').closest('tr').hide();
        return;
    }
    $('.clsAddnewtype_auto').hide();
    var selectedType = new SelectedLawItemType();
    var shtml = GetTextHTMLManual(selectedType.type);   
    $('.clsAddnewtype_manual').html(shtml)
    $('.clsAddnewtype_manual').closest('tr').show();
}
function GetTextHTMLManual(type) {
    var shtml = '';
    var sformatName = GetRowDataHTML(['<b>{0}</b>', '<input class="{1}" type="textbox"  placeholder="{2}" />'], '');
    var sformatContent = GetRowDataHTML(['<b>{0}</b>', '<textarea class="{1}" type="textbox"  placeholder="{2}"  />'], '');


    if (type == 0 || type == 1)//chapter
    {
        shtml += String.format(sformatName, 'Tên:', 'clsAddnewtype_manual_Name', type == 1 ? 'Mục 1' : 'Chương 1.');
        shtml += String.format(sformatName, 'Tiêu đề:', 'clsAddnewtype_manual_Title', type == 1 ? 'ĐỊA GIỚI HÀNH CHÍNH' : 'QUY ĐỊNH CHUNG');
        shtml += String.format(sformatName, 'Số:', 'clsAddnewtype_manual_Number', '1');
    }
    else if (type == 2 || type == 3) {// artical
        if (type == 2)
            shtml += String.format(sformatName, 'Tên:', 'clsAddnewtype_manual_Name', type == 2 ? 'Điều 1' : '1.');
        shtml += String.format(sformatName, 'Tiêu đề:', 'clsAddnewtype_manual_Title', type == 2 ? 'Phạm vi điều chỉnh' : 'QUY ĐỊNH');
        shtml += String.format(sformatName, 'Số:', 'clsAddnewtype_manual_Number', '1');
        shtml += String.format(sformatContent, 'Nội Dung:', 'clsAddnewtype_manual_Content', '');
    }
    shtml = GetTableHTML(shtml, ' clsAddnewLaw_SubTable');
    return shtml;
}
function ValidateForm(isEdit) {
    var selectedType = new SelectedLawItemType();
    var serror = '';
    if (selectedType.content == "") {
        if (selectedType.isAuto || selectedType.type > 1)
            serror += '<p> Vui lòng nhập "Nội Dung"</p>';
    }
    if (!isEdit) {
        if (selectedType.type == 1 && selectedType.chapterID == -1) {
            serror += '<p> Vui lòng chọn "Chương"</p>';
        }
        if (selectedType.type == 2) {
            if (selectedType.chapterID == -1)
                serror += '<p> Vui lòng chọn "Chương"</p>';
            if (selectedType.chapteritemID == -1)
                serror += '<p> Vui lòng chọn "Mục"</p>';
        }
        if (selectedType.type == 3) {// khoan
            if (selectedType.chapterID == -1)
                serror += '<p> Vui lòng chọn "Chương"</p>';
            if (selectedType.chapteritemID == -1)
                serror += '<p> Vui lòng chọn "Mục"</p>';
            if (selectedType.ArticalID == -1)
                serror += '<p> Vui lòng chọn "Điều"</p>';
        }
    }
    if (!selectedType.isAuto) {
        if (selectedType.type == 0 || selectedType.type == 1) {
            if (selectedType.Name == '')
                serror += '<p> Vui lòng nhập "Tên"</p>';
            if (selectedType.Title == '')
                serror += '<p> Vui lòng nhập "Tiêu Đề"</p>';
            if (selectedType.Number < 0)
                serror += '<p> Vui lòng nhập "Số" hợp lệ</p>';
        }
        else {
            if (selectedType.Number < 0)
                serror += '<p> Vui lòng nhập "Số"</p>';
            if (selectedType.Title == '' && selectedType.type == 2 )
                serror += '<p> Vui lòng nhập "Tiêu Đề"</p>';
        }

    }
    return serror;
}
function EditLawItem(type, parentID, ID, childID, iEdit) {
    var scontent = GetContentHTMLEdit(type, parentID, ID, childID);
    scontent = GetTableHTML(GetRowDataHTML(['<div style="text-align:left; color: red" class ="clsAddnewtype_Error"></div>'], 'colspan="2"') + scontent, '');
    var opt = [
        {
            name: 'Update', fClick: function () {
                var serror = ValidateForm(iEdit);
                if (serror != "") {
                    $('.clsAddnewtype_Error').html(serror); $('.clsAddnewtype_Error').closest('tr').show();
                    return;
                } else EmptyError();
                ShowProcessing();
                var selectedType = new SelectedLawItemType();
                selectedType.chapterID = parentID;
                if (type == 0 || type == 1 || type == 2) { 
                    selectedType.chapteritemID = ID;
                }
                if (type == 2)      //artical   
                    selectedType.ArticalID = childID;
                if (type == 3) {     //clause  
                    selectedType.ArticalID = ID;
                    selectedType.ClauseID = childID;
                }
                selectedType.type = type;
                ClosePopup();   
                PageMethods.EditLawItem(selectedType, function (data) {
                    HideProcessing();
                    ReloadMylaw();
                }, function () { HideProcessing(); });
            }  }, {   name: "Cancel", fClick: function () { ClosePopup(); } }
    ];
    ShowPopup("Law Search - Edit", scontent, opt);


}
//type=>0: chuong 1: muc 2: dieu 3: khoan
function AddNewLaw(type, parentID, ID, childID, iEdit) {
    AddNewPopup();
    $('.clsAddnewtype').val(type); $('.clsAddnewtype').change();
    if (type == 0) {
    }
    else if (type == 1) {
        $('.clsAddnewsubtype_Chapter').val(parentID);
    }
    else if (type == 2) {     //artical   
        $('.clsAddnewsubtype_Chapter').val(parentID); $('.clsAddnewsubtype_Chapter').change();
        $('.clsAddnewsubtype_ChapterItem').val(ID);
    }
    else if (type == 3) {     //clause   
        $('.clsAddnewsubtype_Chapter').val(parentID); $('.clsAddnewsubtype_Chapter').change();
        $('.clsAddnewsubtype_ChapterItem').val(ID); $('.clsAddnewsubtype_ChapterItem').change();
        $('.clsAddnewsubtype_Artical').val(childID);
    }
}
function AddNewPopup() {
    var scontent = GetRowDataHTML(['<div style="text-align:left; color: red" class ="clsAddnewtype_Error"></div>'], 'colspan="2"')
        + GetRowDataHTML(['<div style="width:100%; text-align:right;font-weight:bold; font-size:13px">Chọn Loại:</div>', ' <select onchange="addnew_typeChange();" class="clsAddnewtype" ><option value="0"> Chương</option><option value="1"> Mục</option><option value="2"> Điều</option><option value="3"> Khoản</option></select>'], 'class="clsPopupTableTD1"')
    +GetRowDataHTML(['<div class="clsAddnewsubtype"></div>'], 'colspan="2"')
    +GetRowDataHTML(['<div  class="clsAddnewcheckboxExtractAuto" ><input type="checkbox" checked="checked" onchange="checkboxExtractAuto_Changed()" /> Trích xuất tự động</div>'], 'colspan="2"')
    +GetRowDataHTML(['<div class="clsAddnewtype_manual"></div>'], 'colspan="2"')
    +GetRowDataHTML(['<div  class="clsAddnewtype_auto"><b> Nội Dung:</b> <textarea class="clsAddnewtype_content" name="w3review" rows="400" cols="50" ></textarea></div>'], 'colspan="2"');
    scontent = GetTableHTML(scontent,' clsAddnewLaw_Table');
      var opt = [
        {
              name: 'OK', fClick: function () {
                  var serror = ValidateForm(false);
                  if (serror != "") {
                        $('.clsAddnewtype_Error').html(serror); $('.clsAddnewtype_Error').closest('tr').show();
                        return;
                  } else EmptyError();
                  ShowProcessing();
                  var selectedType = new SelectedLawItemType();
                //clsAddnewtype_Error
                PageMethods.AddNewLaw(selectedType, function (data) {
                    HideProcessing();
                    if (data == null | data.mess != null && data.mess != '') {                   
                        $('.clsAddnewtype_Error').html(data == null ? 'Nội dung không hợp lệ.' : data.mess); $('.clsAddnewtype_Error').closest('tr').show();
                        return;
                    }
                    ClosePopup();
                    if (selectedType.isAuto) {
                        //showconfirm
                        var scontent = GetHTMLLawContent(data.law);
                        AddNewLaw_Confirm(scontent);
                    }
                    else 
                        ReloadMylaw();                  
                }, function () { HideProcessing(); });
            },



        }, {
            name: "Cancel", fClick: function () {
                ClosePopup();
            }
        }
    ];
    ShowPopup('Law Search - Add New', scontent, opt);
    EmptyError(); $('.clsAddnewtype_manual').closest('tr').hide();
    addnew_typeChange();
}


function AddNewLaw_Confirm(scontent) {
   
    var opt = [
        {
            name: 'Confirm', fClick: function () {               
                ShowProcessing();
                //clsAddnewtype_Error
                PageMethods.AddNewLaw_Confirm(function (data) {
                    HideProcessing();
                    ClosePopup(); 
                    if (data == "")
                        ReloadMylaw();  
                    else EditFailed(); 
                         
                }, function () { HideProcessing(); });
            },
        }, { name: "Cancel", fClick: function () {  ClosePopup(); }  }
    ];
    ShowPopup('Law Search - Confirm', scontent, opt);
}

function EditFailed() {
    ShowPopup('Law Search - Warning', 'Cập nhật thất bại', null);
}

function addnew_typeChange() {
    var v = $('.clsAddnewtype').val();
    if (v == "0") {
        $('.clsAddnewsubtype').empty();
        $('.clsAddnewsubtype').closest('tr').hide();
    }
    else {
        var shtml = '';
        if (v == "1")
            shtml = GetRowDataHTML(['Mục này thuộc: <b>Chương</b>: '+ GetAllChapterOption() ], '');
        else if (v == "2") 
            shtml = GetRowDataHTML(['Điều này thuộc: '], 'colspan ="2"')
                + GetRowDataHTML(['<b>Chương</b>:&nbsp;&nbsp;' + GetAllChapterOption() ,
            ' <b>Mục:&nbsp;&nbsp;</b> <select class="clsAddnewsubtype_ChapterItem" onchange ="Addnewsubtype_ChapterItem_Change()" ><option value ="-1">-Chọn-</option></select>'], '');      
         
        else if (v == "3") {
            shtml = GetRowDataHTML(['Khoản này thuộc: '], 'colspan ="3"')+
                GetRowDataHTML(['<b> Chương</b>:&nbsp;&nbsp;' + GetAllChapterOption(),
                '<b>Mục: </b>&nbsp;&nbsp;<select class="clsAddnewsubtype_ChapterItem" onchange ="Addnewsubtype_ChapterItem_Change()" ><option value ="-1">-Chọn-</option></select>'
                ,' <b> Điều</b>&nbsp;&nbsp;: <select class="clsAddnewsubtype_Artical"><option value ="-1">-Chọn-</option></select>'
              ], '');
        }
        shtml = GetTableHTML(shtml, ' clsAddnewsubtype_table');
        $('.clsAddnewsubtype').html(shtml);
        $('.clsAddnewsubtype').closest('tr').show();
    }
    
      
    checkboxExtractAuto_Changed();
    EmptyError();
}

function GetAllChapterOption() {
    var shtml = '<select class="clsAddnewsubtype_Chapter" onchange="Addnewsubtype_Chapter_Change();"><option value ="-1">-Chọn-</option>';
    var mylaw = GetMylaw();
    if (mylaw != null) {
        for (var i = 0; i < mylaw.lstChapters.length; i++) {
            var chapter = mylaw.lstChapters[i];
            shtml += '<option value ="' + chapter.ID + '">' + chapter.Name + '</option>';

        }
    }
    shtml += '</select>';
    return shtml;
}
function Addnewsubtype_Chapter_Change() {
    if ($('.clsAddnewtype').val() < 2) return;

    var shtml = '<option value ="-1">-Chọn-</option>';
    var chapterID = $('.clsAddnewsubtype_Chapter').val();
    var mylaw = GetMylaw();
    if (mylaw != null) {
        for (var i = 0; i < mylaw.lstChapters.length; i++) {
            var chapter = mylaw.lstChapters[i];
            if (chapter.ID != chapterID) continue;
            if (chapter.lstChapterItem.length > 0)
                for (var j = 0; j < chapter.lstChapterItem.length; j++) {
                    var name = chapter.lstChapterItem[j].Name;
                    shtml += '<option value ="' + chapter.lstChapterItem[j].ID + '">' + (name == '' ? 'Mặc định' : name) + '</option>';
                }
            else
                shtml += '<option value ="0">Mặc định</option>';
            break;
        }
    }


    $('.clsAddnewsubtype_ChapterItem').html(shtml);
    checkboxExtractAuto_Changed();
    Addnewsubtype_ChapterItem_Change();

    
}
function Addnewsubtype_ChapterItem_Change() {
    if ($('.clsAddnewtype').val() < 3) return;

    var shtml = '<option value ="-1">-Chọn-</option>';
    var chapterID = $('.clsAddnewsubtype_Chapter').val();
    var chapteritemID = $('.clsAddnewsubtype_ChapterItem').val();
    var mylaw = GetMylaw();
    if (mylaw != null) {
        for (var i = 0; i < mylaw.lstChapters.length; i++) {
            var chapter = mylaw.lstChapters[i];
            if (chapter.ID != chapterID) continue;
            for (var j = 0; j < chapter.lstChapterItem.length; j++) {

                if (chapter.lstChapterItem[j].ID != chapteritemID) continue;
                var chapterItem = chapter.lstChapterItem[j];
                for (var k = 0; k < chapterItem.lstArtical.length; k++) {
                    var artical = chapterItem.lstArtical[k];
                    shtml += '<option value ="' + artical.ID + '">' + artical.Name + '</option>';
                }

            }

            break;
        }

    }


    $('.clsAddnewsubtype_Artical').html(shtml);
    checkboxExtractAuto_Changed();
    EmptyError();
}
function EmptyError() {
    $('.clsAddnewtype_Error').empty();
    $('.clsAddnewtype_Error').closest('tr').hide();
}