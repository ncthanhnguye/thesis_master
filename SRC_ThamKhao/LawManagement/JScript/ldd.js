$(document).ready(function () {
    $('.my-nav').empty();
    isEdit = $('[id$=hdfIsEdit]').val() == '1';
    ReloadMylaw();
});
var lstLaw = null;
var isEdit = false;
function ViewLawDetails() {
    var lstArticalID = new Array();
    if ($('[id$=cboDieu]').val() == -1) {
        $('[id$=cboDieu] option').each(function () {
            if ($(this).val() != -1)
                lstArticalID.push($(this).val());
        });
    }
    else
        lstArticalID.push($('[id$=cboDieu]').val());
    ViewLawByListArtical(lstArticalID, -1, true);

}
function ViewLawByListArtical(lstArticalID, chapterID, isPopup) {   
    ShowProcessing();
    var LawID = $('[id$=cboLuat]').val();
    PageMethods.ViewDetailArticalByView(lstArticalID, chapterID, LawID, function (data) {
        HideProcessing();
        if (isPopup)
            ShowDetailArtical([data]);
        else {
            var scontent = GetHTMLLawContent(data);
            $('.divlawEditDetail').html(scontent);
        }
    }, function () {
        HideProcessing();
    });
}

function GetMylaw() {
    var mylaw = null;
    for (var i = 0; lstLaw != null && i < lstLaw.length; i++) {
        if (lstLaw[i].ID == $('[id$=cboLuat]').val()) {
            mylaw = lstLaw[i];
            break;
        }
    }
    return mylaw;
}
function ReloadMylaw() {
  
    if (lstLaw != null) {
        var k = -1;
        for (var i = lstLaw.length - 1; i >= 0; i--) {
            if (lstLaw[i].ID == $('[id$=cboLuat]').val()) {
                k = i;
               
                break;
            }
        }
        if (k >= 0)
            lstLaw.splice(k, 1);
    }
    $('[id$=btnView]').click();
}
function ViewLaw() {
    try {
        if (lstLaw == null) lstLaw = new Array();
        var mylaw = GetMylaw();

        if (mylaw == null) {
            ShowProcessing();
            PageMethods.LoadLawData($('[id$=cboLuat]').val(), function (data) {
                HideProcessing();
                var o = data;
                lstLaw.push(data);
                ShowLaw(data);

            }, function () { HideProcessing(); });
        }
        else ShowLaw(mylaw);

    } catch (e) {

    }
}
function ViewMuc(chapterID, chapterItemID) {
    var mylaw = GetMylaw(); var lstArticalID = new Array(); var findtext_Chuong, findtext_Muc = '', findtext_Dieu;
    if (mylaw != null) {
        for (var i = 0; i < mylaw.lstChapters.length; i++) {
            var chapter = mylaw.lstChapters[i];
            if (chapter.ID != chapterID) continue;
            findtext_Chuong = chapter.Name;
            for (var j = 0; j < chapter.lstChapterItem.length; j++) {
                if ($('[id$=cboMuc]').val() != -1 && $('[id$=cboMuc]').val() != chapter.lstChapterItem[j].ID || (chapterItemID > 0 && chapterItemID != chapter.lstChapterItem[j].ID))
                    continue;
                findtext_Muc = chapter.lstChapterItem[j].Name + '.';
                for (var k = 0; k < chapter.lstChapterItem[j].lstArtical.length; k++) {
                    lstArticalID.push(chapter.lstChapterItem[j].lstArtical[k].ID);
                }
                break;
            }
            if (findtext_Muc != '') break;
        }
        if (isEdit)
            ViewLawByListArtical(lstArticalID, chapterID, false);
        else
            ScrollToFindText(findtext_Muc + '.', 'p a b span');
    }
}
function ViewChuong(chapterID) {
    var mylaw = GetMylaw(); var lstArticalID = new Array(); var findtext_Chuong = '', findtext_Muc, findtext_Dieu;
    if (mylaw != null) {
        for (var i = 0; i < mylaw.lstChapters.length; i++) {
            var chapter = mylaw.lstChapters[i];
            if (chapter.ID != chapterID) continue;
            findtext_Chuong = chapter.Name;          
            for (var j = 0; j < chapter.lstChapterItem.length; j++) {
                for (var k = 0; k < chapter.lstChapterItem[j].lstArtical.length; k++) {
                    lstArticalID.push(chapter.lstChapterItem[j].lstArtical[k].ID);
                }
            }
            break;
        }
    }
    if (isEdit)
            ViewLawByListArtical(lstArticalID, chapterID, false);
    else ScrollToFindText(findtext_Chuong, 'p span');
}

function ViewDieu(ArticalID) {
    var mylaw = GetMylaw(); var findtext_Dieu = ''; var lstArticalID = new Array(); 
    for (var i = 0; i < mylaw.lstChapters.length; i++) {
        var chapter = mylaw.lstChapters[i];
        if ($('[id$=cboChuong]').val() != -1 && $('[id$=cboChuong]').val() != chapter.ID)
            continue;

        for (var j = 0; j < chapter.lstChapterItem.length; j++) {
            if ($('[id$=cboMuc]').val() != -1 && $('[id$=cboMuc]').val() != chapter.lstChapterItem[j].ID)
                continue;
            for (var k = 0; k < chapter.lstChapterItem[j].lstArtical.length; k++) {
                var artical = chapter.lstChapterItem[j].lstArtical[k];
                if (artical.ID != ArticalID)
                    continue;
                lstArticalID.push(ArticalID);
                findtext_Dieu = artical.Name;
                break;
                // lstArticalID.push(artical.ID);
            }
            if (findtext_Dieu != '') break;
        } if (findtext_Dieu != '') break;
    }
    if (isEdit)
        ViewLawByListArtical(lstArticalID, -1, false);
    else
        ScrollToFindText(findtext_Dieu + '.', 'p  a  b  span');
}
//p:nth-child(535) > a > b > span
function ScrollToFindText(v1, findPath) {
    $('.divlawDetail').find(findPath).each(function () {
        var v = $(this).text().trim();
        if (v != '') {
            if (v.toLowerCase().indexOf(v1.toLowerCase()) == 0)
                $(this)[0].scrollIntoView();
            //   scroll($(this)[0], $('.divlawDetail'));
        }
    });
}
function scroll(element, parent) {
    $(parent).animate({ scrollTop: $(element).offset().top - $(parent).offset().top }, { duration: 'slow', easing: 'swing' });
}
function ShowLaw(data) {
    var menu = '';
    for (var i = 0; i < data.lstChapters.length; i++) {
        var chapter = data.lstChapters[i];
        if ($('[id$=cboChuong]').val() != -1 && $('[id$=cboChuong]').val() != chapter.ID)
            continue;
        menu += ' <li><a href="#" title="" class="caret"  onclick ="ViewChuong(' + chapter.ID + ')" ><b>' + chapter.Name + '</b> ' + chapter.Title + '</a>' + GetEditDelete(0, chapter.ID, chapter.ID,0);
        if (chapter.lstChapterItem.length > 1 || chapter.lstChapterItem.length == 1 && chapter.lstChapterItem[0].Number > 0) {
            menu += GetListChapterItem(chapter.lstChapterItem);
        }
        else {
            if (chapter.lstChapterItem == null || chapter.lstChapterItem.length == 0) {
                if (isEdit) {
                    menu += ' <ul class="nested" ><li><div class="clsButtonAdd" onclick="AddNewLaw(1,' + chapter.ID + ',0,0,0);" >Mục Mới</div></li><li><div class="clsButtonAdd" onclick="AddNewLaw(2,' + chapter.ID + ',0,0,0);" >Điều Mới</div></li></ul>';
                }
              } else
            menu += GetListArtical(chapter.lstChapterItem.length > 0 ? chapter.lstChapterItem[0] : null);
        }
        menu += '</li>';
    }

    $('.my-nav').html(menu);
    // $('.my-nav').mgaccordion();
    var toggler = document.getElementsByClassName("caret");
    var i;

    for (i = 0; i < toggler.length; i++) {
        toggler[i].addEventListener("click", function () {
            this.parentElement.querySelector(".nested").classList.toggle("active");
            this.classList.toggle("caret-down");
        });
    }
}
function GetListChapterItem(lstChapterItem) {
    var menu = '<ul class="nested">';
    for (var i = 0; lstChapterItem != null && i < lstChapterItem.length; i++) {
        if ($('[id$=cboMuc]').val() != -1 && $('[id$=cboMuc]').val() != lstChapterItem[i].ID)
            continue;
        menu += ' <li><a href="#" title="" class="caret"  onclick ="ViewMuc(' + lstChapterItem[i].ChapterID + ',' + lstChapterItem[i].ID + ')"> ' + lstChapterItem[i].Name + ' ' + lstChapterItem[i].Title+ '</a>' + GetEditDelete(1, lstChapterItem[i].ChapterID, lstChapterItem[i].ID,0);
        menu += GetListArtical(lstChapterItem[i]);
        menu += ' </li>';
    }
    if (isEdit) {
        if (lstChapterItem != null && lstChapterItem.length > 0)
            menu += '<li ><div class="clsButtonAdd" onclick="AddNewLaw(1,' + lstChapterItem[0].ChapterID + ',0,0,0);" >Mục mới</div></li>';      
    }
    menu += '</ul>';
    return menu;
}

function GetListArtical(chapterItem) {
    var menu = '<ul class="nested">';
    for (var i = 0; chapterItem.lstArtical != null && i < chapterItem.lstArtical.length; i++) {
        var artical = chapterItem.lstArtical[i];
        if ($('[id$=cboDieu]').val() != -1 && $('[id$=cboDieu]').val() != artical.ID)
            continue;
        menu += ' <li><a href="#" title="" class="caret"  onclick ="ViewDieu(' + artical.ID + ');" ><b>' + artical.Name + '</b> ' + artical.Title + '</a>' + GetEditDelete(2, chapterItem.ChapterID, chapterItem.ID, artical.ID);
        menu += GetListClauseItem(artical);
        menu += ' </li>';
    }
    if (isEdit)
        menu += '<li ><div class="clsButtonAdd" onclick="AddNewLaw(2,' + chapterItem.ChapterID + ',' + chapterItem.ID + ',0,0);">Điều mới</div></li>';
    menu += '</ul>';
    return menu;
}
function GetListClauseItem(artical) {

    var menu = '<ul class="nested">';
    for (var i = 0; artical.lstClause != null &&i < artical.lstClause.length; i++) {
        var clause = artical.lstClause[i];
        menu += ' <li><a href="#" title="" onclick ="ViewDieu(' + artical.ID + ')"> Khoản ' + clause.Number + '</a>' + GetEditDelete(3, artical.ChapterID , artical.ID, clause.ID);
        //menu += GetListLawItem(chapter.lstChapterItem[i], ul == 'ul' ? 'li' : 'ul');
        menu += ' </li>';
    }
    if (artical.lstClause.length == 0) menu += ' <li></li>';
    if (isEdit)
        menu += '<li ><div class="clsButtonAdd" onclick="AddNewLaw(3,' + artical.ChapterID + ',' + artical.ChapterItemID + ',' + artical.ID + ',0);">Khoản mới</div></li>';
    menu += '</ul>';
    return menu;
}
function GetEditDelete(type, parentID, ID, childID) {
    if (!isEdit) return '';

    if (type == 3)// clause
        return '<a class="clsEditDeleteButton" onclick = "DeleteLaw(' + type + ',' + ID + ',' + childID + ')">Xóa</a> <a class="clsEditDeleteButton" onclick = "EditLawItem(' + type + ',' + parentID + ',' + ID + ',' + childID + ',1)">Sửa</a>';
    if (type == 2)// artical
        return '<a class="clsEditDeleteButton" onclick = "DeleteLaw(' + type + ',' + ID + ',' + childID + ')">Xóa</a> <a class="clsEditDeleteButton" onclick = "EditLawItem(' + type + ',' + parentID + ',' + ID + ',' + childID + ',1)">Sửa</a>';
    if (type == 1)// chapterItem
        return '<a class="clsEditDeleteButton" onclick = "DeleteLaw(' + type + ',' + parentID + ',' + ID + ')">Xóa</a> <a class="clsEditDeleteButton" onclick = "EditLawItem(' + type + ',' + parentID + ',' + ID + ',' + childID + ',1)">Sửa</a>';
    if (type == 0)// chapter
        return '<a class="clsEditDeleteButton" onclick = "DeleteLaw(' + type + ',' + parentID + ',' + ID + ')">Xóa</a> <a class="clsEditDeleteButton" onclick = "EditLawItem(' + type + ',' + parentID + ',' + ID + ',' + childID + ',1)">Sửa</a>';
}

