var order = 1;
function SearchLaw() {
   
    var textsearch = $('[id$=txtSearch]').val();
    if ( textsearch.trim() == '' || textsearch.trim().length < 12) {
        var opt = [{
            name: "Tiếp tục", fClick: function () {
                ClosePopup();
            }
        }, {
            name: "Hủy", fClick: function () {
                ClosePopup();
            }, className: "button-light"
        }];
        ShowPopup('Thông Báo', 'Vui lòng nhập thông tin cần tìm kiếm lớn hơn 12 ký tự', null);
    }
    else {
        $('.divContentHTMLLaw').empty();
        ShowProcessing();
        PageMethods.SearchLawByText(textsearch, function (rs) {
            HideProcessing();
            if (rs == null) {

                Nodata();
                return;
            }
            var scontent = '';
            var sformat = ' <div class="g " lang="vi" style="width: 100%"  >'
                + '<div><div ><div style="  display: flex; padding: 5px;" ><input type ="checkbox" articalID ="{2}" style="margin: 5px; display:none"><h3 style="text-align:center; valign:bottom; padding:5px;line-height: 1.58;">{3}. </h3>  <a onclick="ViewDetailArtical([{2}]);" title="Xem chi tiết..."  ><h3 class="LC20lb MBeuO DKV0Md">{0}</h3></a></div></div>'
                + ' <div> <div><span style="padding-left:25px;">{1}&nbsp;...</span><span style="color:red">(score:{4})</span></div></div></div> </div>';

            for (var i = 0; i < rs.lstArticals.length; i++) {
                var artical = rs.lstArticals[i];
                scontent += String.format(sformat, artical.Title, GetMaxLenth(artical.Content, 100), artical.ID, order++, artical.distance);
                scontent += '';
            }
            //for (var ii = 0; ii < rs.laws.length; ii++) {
            //    var data  = rs.laws[ii];
            //    var scontent = ''; order = 1;
            //    for (var i = 0; i < data.lstChapters.length; i++) {

            //        var chapter = data.lstChapters[i];
            //        var lstChapterItem = chapter.lstChapterItem;
            //        for (var j = 0; j < lstChapterItem.length; j++) {
            //            scontent += ViewListArtical_Result(lstChapterItem[j], chapter.Name);
            //        }
            //        scontent += '';

            //    }
            //}
           
            $('.sresult').text((order-1).toString() +' kết quả');// kết quả
            $('.divContentHTMLLaw').html(scontent);
         
        });
    }
}
function Nodata() {
    $('.sresult').text('');// kết quả
    ShowPopup('Thông Báo', 'Không tìm thấy kết quả phù hợp.', null);
}
function ViewListArtical_Result(chapterItem, chapterName) {
    var scontent = '';
    var sformat = ' <div class="g " lang="vi" style="width: 100%"  >'
        + '<div><div ><div style="  display: flex; padding: 5px;" ><input type ="checkbox" articalID ="{2}" style="margin: 5px; display:none"><h3 style="text-align:center; valign:bottom; padding:5px;line-height: 1.58;">{3}. </h3>  <a onclick="ViewDetailArtical([{2}]);" title="Xem chi tiết..."  ><h3 class="LC20lb MBeuO DKV0Md">{0}</h3></a></div></div>'
        + ' <div> <div><span style="padding-left:25px;">{1}&nbsp;...</span><span style="color:red">(score:{4})</span></div></div></div> </div>';
    for (var i = 0; i < chapterItem.lstArtical.length; i++) {
        var artical = chapterItem.lstArtical[i];
        scontent += String.format(sformat, chapterName + ' - ' + artical.Name, GetMaxLenth(artical.Title, 100), artical.ID, order++, artical.score);
    }
 
    return scontent;
}
function GetMaxLenth(s, total) {
    if (s.length < total) return s;
    return s.substr(0, total);
}
function ViewDetailArtical(lstArticalID) {
    if (lstArticalID == null || lstArticalID.length == 0) {
        ShowDetailArtical('');
        return;
    }
    ShowProcessing();
    PageMethods.ViewDetailArticalByID_Result(lstArticalID,  function (data) {
        HideProcessing();
        ShowDetailArtical(data);
    }, function () { HideProcessing(); });
}

function GetHTMLLawContent(data) {
    if (data == null) return '';
    var o = data;
    var scontent = '';
    for (var i = 0; i < data.lstChapters.length; i++) {
        var chapter = data.lstChapters[i];
        var lstChapterItem = chapter.lstChapterItem;
        for (var j = 0; j < lstChapterItem.length; j++) {

            scontent += ViewListArtical_Detail(lstChapterItem[j], chapter.Name, chapter.Title);
        }
        scontent += '';

    }
    scontent = ($('[id$=txtSearch]').length == 0 ? '' : '<div class="divQuestion">Nội dung tìm kiếm: <b>' + $('[id$=txtSearch]').val() + '</b></div>') + '<div class="divlawDetail">' + scontent + '</div>';
    return scontent;
}
function ShowDetailArtical(rs) {
    var scontent = '';// GetHTMLLawContent(data);

    for (var i = 0; rs != null && rs != '' && i < rs.length; i++)
        scontent += GetHTMLLawContent(rs[i]);
    ShowPopup('Thông Tin Chi Tiết', scontent, null);
}
function ViewListArtical_Detail(chapterItem, chapterName, chapterTitle) {
    var scontent = GetChapterDisplay(chapterName, chapterTitle);
    if (chapterItem.Number > 0) {
        scontent += '<p style="margin-top:6.0pt"><a ><b><span lang="VI">' + chapterItem.Name + '. ' + chapterItem.Title + '</span></b></a></p>';
    }
    var sFormat_ArticalHeader = '<p style="margin-bottom: 6.0pt; text-align: justify"><a><b><span style="letter-spacing: 1.2pt">{0}</span></b></a><b><span style="letter-spacing: 1.2pt"> </span></b><a>{1}</a></p>'
    var sformat_Artical_Content = '<p style="margin-bottom: 6.0pt; text-align: justify">{0}</p >';

    for (var i = 0; i < chapterItem.lstArtical.length; i++) {
        var artical = chapterItem.lstArtical[i];
       
        if (artical.lstClause.length == 0) {
            scontent += String.format(sformat_Artical_Content, artical.ArticalContent.replace(artical.Name + '.', '<b>' + artical.Name + '.'+'</b>'));
        }
        else {
            scontent += String.format(sFormat_ArticalHeader, artical.Name, artical.Title);
            for (var j = 0; j < artical.lstClause.length; j++) {
                var clause = artical.lstClause[j];
                if (clause.lstPoints.length == 0)
                    scontent += String.format(sformat_Artical_Content, clause.Content);
                else {
                    scontent += String.format(sformat_Artical_Content, clause.Title);
                    for (var k = 0; k < clause.lstPoints.length; k++) {
                        var point = clause.lstPoints[k];
                    
                        scontent += String.format(sformat_Artical_Content, point.Content);
                    }
                }
            }
        }
    }

    return scontent;
}

function GetChapterDisplay(chapterName, chapterTitle) {
    var sFormat = '<p align="center" style="margin-bottom: 6.0pt; text-align: center"><a><b><span style="letter-spacing: 1.4pt">{0}</span></b></a></p><p align="center" style="margin-bottom: 6.0pt; text-align: center"><a ><b><span style="letter-spacing: 1.4pt">{1}</span></b></a></p>';
    return String.format(sFormat, chapterName, chapterTitle)
}